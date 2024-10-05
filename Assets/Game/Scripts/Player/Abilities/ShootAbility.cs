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
            
            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
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