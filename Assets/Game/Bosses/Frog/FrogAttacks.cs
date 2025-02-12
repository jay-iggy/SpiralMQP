using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts {
    public class FrogAttacks : MonoBehaviour {
        [Header("Movement")]
        [SerializeField] private float speed = 10;
        [SerializeField] private float moveDistanceThreshold = 3;
        public float jumpForce = 20f;
        public float rotateSpeed = 5f;
        public float delayBetweenJumps = 1f;
        
        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int projectileCount = 8;
        [SerializeField] private float projectileCircleRadius = 2;
        [SerializeField] private float projectileSpeed = 10;
        
        private bool _isGrounded = false;
        
        private MovementComponent _movementComponent;
        private HealthComponent _healthComponent;
        private GameObject _player;
        
        private List<GameObject> _itemsToCleanup = new();
        
        private void Awake() {
            _movementComponent = GetComponent<MovementComponent>();
            _healthComponent = GetComponent<HealthComponent>();
        }
        private void Start() {
            _player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            StartCoroutine(FrogEnemyBehavior());
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
        
        #region Behavior
            private IEnumerator FrogEnemyBehavior() {
                print("FrogEnemyBehavior started");
                while (true) {
                    Vector3 targetPosition = BossRoom.GetRandomPositionInRoom(5);
                    yield return RotateToFace(targetPosition);
                    yield return JumpTo(targetPosition);
                    yield return new WaitForSeconds(delayBetweenJumps);
                    targetPosition = _player.transform.position;
                    yield return RotateToFace(targetPosition);
                    yield return JumpTo(targetPosition);
                    yield return new WaitForSeconds(delayBetweenJumps);
                }
            }
            private void OnHealthChanged(float newHealth) {
                
            }
        #endregion
        
        
        
        IEnumerator JumpTo(Vector3 target) {
            _isGrounded = false;
            _movementComponent.AddVerticalVelocity(jumpForce);
            _movementComponent.moveVelocity = (target - transform.position).normalized * speed;
            while (!_isGrounded) {
                yield return null;
            }
        }
        
            
        private IEnumerator RotateToFace(Vector3 target) {
            float timer=0;
            while (Vector3.Angle(transform.forward, target - transform.position) > 10f){
                Vector3 targetDir = target - transform.position;
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }


        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag(TagManager.Ground)) {
                _movementComponent.moveVelocity = Vector3.zero;
                GameObject[] projectiles = new GameObject[projectileCount];
                for(int i = 0; i < projectileCount; i++) {
                    projectiles[i] = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                }
                BulletPatterns.CreateCircle(projectiles, transform.position, projectileCircleRadius);
                foreach (GameObject projectile in projectiles) {
                    projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.position - transform.position).normalized * projectileSpeed;
                }
                _isGrounded = true;
            }
            else if (other.gameObject.CompareTag(TagManager.Player)) {
                MovementComponent playerMovementComponent = other.gameObject.GetComponent<MovementComponent>();
                if (playerMovementComponent != null) {
                    //move the player to the side so the frog can land
                    Vector3 direction = (other.transform.position - transform.position).normalized;
                    direction.y = 0;
                    playerMovementComponent.AddExternalVelocity(direction * 4);
                }
            }
        }
    }
}
