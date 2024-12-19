using System;
using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class ShotgunAttacks : MonoBehaviour {
        [Header("Movement")]
        [SerializeField] private float speed = 10;
        [SerializeField] private float moveDistanceThreshold = 3;
        public float jumpForce = 10f;
        private bool _lockXRotation = true;
        [Header("Shooting")]
        [SerializeField] GameObject projPrefab;
        [SerializeField] private Transform projSpawnPos;
        [SerializeField] private int volleysPerAttack = 2; // increases during phase 2
        [SerializeField] private float delayBetweenVolleys = .5f;
        [SerializeField] private float aimRotateSpeed = 2;
        [SerializeField] private float maxAimTime = 1.5f;
        [SerializeField] private float knockbackForce = 50;
        private bool _fired = false;
        [Header("Shells")]
        [SerializeField] private GameObject shellPrefab;
        [SerializeField] private List<Transform> shellEjectPositions;
        [SerializeField] private float shellEjectForce = 20;
        [Header("Animation Clips")]
        [SerializeField] private AnimationClip ejectShellsAnim;
        [SerializeField] private AnimationClip shootAnim;
        
        private Animator _animator;
        private MovementComponent _movementComponent;
        private HealthComponent _healthComponent;
        private GameObject _player;
        
        private List<GameObject> _itemsToCleanup = new();
        
        private void Awake() {
            _animator = GetComponent<Animator>();
            _movementComponent = GetComponent<MovementComponent>();
            _healthComponent = GetComponent<HealthComponent>();
        }
        private void Start() {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            StartCoroutine(ShotgunEnemyBehavior());
        }
        private void OnEnable() {
            _healthComponent.onHealthChanged.AddListener(OnHealthChanged);
        }
        private void OnDestroy() {
            foreach (GameObject item in _itemsToCleanup) {
                if(item != null) {
                    Destroy(item);
                }
            }
        }
        
        private void Update() {
            _animator.SetFloat("WalkSpeed", _movementComponent.moveVelocity.magnitude);
        }
        
        #region Behavior
            private IEnumerator ShotgunEnemyBehavior() {
                print("ShotgunEnemyBehavior started");
                while (true) {
                    yield return MoveToPoint(BossRoom.GetRandomPositionInRoom(5));
                    yield return Attack_Shoot();
                    yield return Attack_EjectShells();
                    yield return new WaitForSeconds(1);
                }
            }
            private void OnHealthChanged(float newHealth) {
                // enter phase 2 if less than half health
                if (newHealth/_healthComponent.maxHealth < .5f) {
                    volleysPerAttack = 6;
                    _healthComponent.onHealthChanged.RemoveListener(OnHealthChanged); // prevent phase changing again
                }
            }
        #endregion
        
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
            _movementComponent.moveVelocity = Vector3.zero;
        }

        #region Shell Eject Attack
            private IEnumerator Attack_EjectShells() {
                _animator.SetTrigger("EjectShells");
                yield return new WaitForSeconds(ejectShellsAnim.length);
            }
            public void EjectShells() { // this is invoked by animation event
                for (int i = 0; i < volleysPerAttack; i+=2) {
                    foreach (Transform ejectPos in shellEjectPositions) {
                        GameObject shell = Instantiate(shellPrefab, ejectPos.position, shellPrefab.transform.rotation);
                        shell.GetComponent<Rigidbody>().AddForce(ejectPos.forward * shellEjectForce, ForceMode.Impulse);
                        _itemsToCleanup.Add(shell);
                    }
                }
            }
        #endregion

        #region Shooting
            private IEnumerator Attack_Shoot() {
                for(int i = 0; i < volleysPerAttack; i++) {
                    _fired = false;
                    yield return RotateToFacePlayer();
                    if(!_fired) {
                        _animator.SetTrigger("Shoot");
                        yield return new WaitForSeconds(shootAnim.length + delayBetweenVolleys);
                    }
                }
            }
            public void ShootVolley() { // this is invoked by animation event
                GameObject proj = Instantiate(projPrefab, projSpawnPos.position, projSpawnPos.rotation);
                _itemsToCleanup.Add(proj);
                _movementComponent.AddExternalVelocity(transform.forward * -knockbackForce);
            }
            
            public void UnlockXRotation() => _lockXRotation = false; // this is invoked by animation event
            public void Jump() { // this is invoked by animation event
                _movementComponent.AddVerticalVelocity(jumpForce);
            }
            private IEnumerator RotateToFacePlayer() {
                float timer=0;
                while (Vector3.Angle(transform.forward, _player.transform.position - transform.position) > 10f){
                    Vector3 targetDir = _player.transform.position - transform.position;
                    float step = aimRotateSpeed * Time.deltaTime;
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                    Vector3 origRot = transform.eulerAngles;
                    float x;
                    // Jump shot: x rotation
                    if (_lockXRotation) {
                        x = 0;
                    } else {
                        x = origRot.x;
                    }
                    transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, origRot.z);
                    yield return null;
                    // Jump shot: if aiming for too long, jump and shoot
                    if (timer > -1) {
                        timer += Time.deltaTime;
                    }
                    if (timer > maxAimTime) {
                        _animator.SetTrigger("JumpShot");
                        timer = -1;
                        _fired = true;
                    }
                }
                _lockXRotation = true;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        #endregion
    }
}
