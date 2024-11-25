using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Player.Abilities {
    public class ShootAbility : Ability {
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;
        public float cooldown = 0.33f;
        public Projectile projectilePrefab;
        public bool isAutomatic = false;
        private bool isHolding = false;
        private float _cooldownTimer = 0;

<<<<<<< Updated upstream
=======
        //audio
        public Sound sfx;
        
>>>>>>> Stashed changes

        public override void AbilityPressed() {
            Shoot();
            
            isHolding = true;
        }

        public override void AbilityReleased() {
            isHolding = false;
        }
        
        private void Update() {
<<<<<<< Updated upstream
            if(_cooldownTimer > 0) {
=======
            if (_cooldownTimer > 0) {
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            
            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
=======

            PlaySound();

            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.transform.position += transform.TransformDirection(spawnOffset);
>>>>>>> Stashed changes
            projectile.dmg = projectileDamage;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = _player.transform.forward * projectileSpeed;
            
            _cooldownTimer = cooldown;
        }
        
        private bool CanShoot() {
            return _cooldownTimer <= 0;
        }
    }
}