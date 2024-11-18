using System;
using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public class ShotgunAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] GameObject projPrefab;
        private GameObject player;
        private int _ammoCount = 2;
        private Projectile bulletInChamber;

        private Vector3 center = new Vector3(0, 0, 0);
        [SerializeField]private float speed = 10;
        List<Projectile> bullets = new();

        private int curAttack = -1;

        [SerializeField] private Transform projSpawnPos;
        
        [SerializeField] private GameObject shellPrefab;
        [SerializeField] private List<Transform> shellEjectPositions;
        [SerializeField] private float shellEjectForce = 20;
        
        [SerializeField] private AudioClip shootSFX;
        
        [SerializeField] private AnimationClip ejectShellsAnim;
        [SerializeField] private AnimationClip shootAnim;
        
        private Animator _animator;
        private MovementComponent _movementComponent;
        
        [SerializeField] private float moveDistanceThreshold = 3;

        private void Awake() {
            _animator = GetComponent<Animator>();
            _movementComponent = GetComponent<MovementComponent>();
        }
        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            StartCoroutine(ShotgunEnemyBehavior());
        }

        private void Update() {
            _animator.SetFloat("WalkSpeed", _movementComponent.moveVelocity.magnitude);
        }
        
        public int GetAttackCount() { return 1; }

        public float Attack(int index) {
            return 0;
        }

        private IEnumerator ShotgunEnemyBehavior() {
            print("ShotgunEnemyBehavior started");
            while (true) {
                yield return MoveToPoint(BossRoom.GetRandomPositionInRoom(3));
                yield return new WaitForSeconds(1);
                yield return Attack_Shoot();
                yield return new WaitForSeconds(1);
                yield return Attack_EjectShells();
                yield return new WaitForSeconds(1);
            }
        }
        
        private IEnumerator MoveToPoint(Vector3 point) {
            while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), point) > moveDistanceThreshold) {
                _movementComponent.moveVelocity = (point - transform.position).normalized * speed;
                // rotate along y axis to face the move velocity direction
                Vector3 targetDir = _movementComponent.moveVelocity;
                float step = 2 * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                yield return null;
            }
        }
        
        
        private IEnumerator Attack_EjectShells() {
            _animator.SetTrigger("EjectShells");
            yield return new WaitForSeconds(ejectShellsAnim.length);
        }
        public void EjectShells() { // this is invoked by animation event
            for (int i = 0; i < volleysPerAttack; i+=2) {
                foreach (Transform ejectPos in shellEjectPositions) {
                    GameObject shell = Instantiate(shellPrefab, ejectPos.position, shellPrefab.transform.rotation);
                    shell.GetComponent<Rigidbody>().AddForce(ejectPos.forward * shellEjectForce, ForceMode.Impulse);
                }
            }
        }
        
        [SerializeField] private int volleysPerAttack = 2; // increases during phase 2
        [SerializeField] private float delayBetweenVolleys = .5f;
        
        private IEnumerator RotateToFacePlayer() {
            while (Vector3.Angle(transform.forward, player.transform.position - transform.position) > 10f){
                Vector3 targetDir = player.transform.position - transform.position;
                float step = 2 * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                yield return null;
            }
        }

        private IEnumerator Attack_Shoot() {
            // make sure it rotates to face the player
            for(int i = 0; i < volleysPerAttack; i++) {
                yield return RotateToFacePlayer();
                _animator.SetTrigger("Shoot");
                yield return new WaitForSeconds(shootAnim.length + delayBetweenVolleys);
            }
        }
        
        [SerializeField] private float knockbackForce = 10;
        
        public void ShootVolley() { // this is invoked by animation event
            GameObject proj = Instantiate(projPrefab, projSpawnPos.position, projSpawnPos.rotation);
            _movementComponent.AddExternalVelocity(transform.forward * -knockbackForce);
        }
    }

}
