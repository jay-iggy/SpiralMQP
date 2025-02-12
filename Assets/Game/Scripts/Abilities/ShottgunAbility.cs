using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Player.Abilities {
    public class ShottgunAbility : AttackAbility {
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;
        public float cooldown = 0.33f;
        public Projectile projectilePrefab;
        public bool isAutomatic = false;
        public Vector3 spawnOffset = new Vector3(0, 1, 0);
        private bool isHolding = false;
        private float _cooldownTimer = 0;
        private int _nextProjID = 0;

        //audio
        public Sound sfx;

        private void Start()
        {
            baseDamage = projectileDamage;
        }

        public override void AbilityPressed() {
            Shoot();
            
            isHolding = true;
        }

        public override void AbilityReleased() {
            isHolding = false;
        }


        private void Update() {
            if(_cooldownTimer > 0) {
                _cooldownTimer -= Time.deltaTime;
            }
            
            if (isHolding && isAutomatic) {

                Shoot();

            }
        }
        
        private void Shoot() {
            if (!CanShoot()) {
                return;
            }

            onAttack.Invoke();
            PlaySound();

            for (int i = 0; i < 6; i++)
            {
                Projectile projectile = Instantiate(projectilePrefab, transform.position + transform.TransformDirection(spawnOffset), Quaternion.identity);
                projectile.dmg = CalculateDamage();
                projectile.projID = _nextProjID;
                projectile.IgnoreInvincibility();
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                float randomAngle = Random.Range(-30f, 30f);

                // Create a rotation around the Y-axis (assuming you're working in 3D space)
                Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

                // Apply the random rotation to the player's forward direction
                Vector3 randomDirection = randomRotation * _player.transform.forward;

                // Set the velocity of the rigidbody
                rb.velocity = randomDirection * projectileSpeed;

            }

            _nextProjID++;
                _cooldownTimer = cooldown;

        }
             
        private bool CanShoot() {
            return _cooldownTimer <= 0;
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}