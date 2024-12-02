using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Player.Abilities {
    public class ShootAbility : AttackAbility {
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;
        public float cooldown = 0.33f;
        public Projectile projectilePrefab;
        public bool isAutomatic = false;
        public Vector3 spawnOffset = new Vector3(0, 1, 0);
        private bool isHolding = false;
        private float _cooldownTimer = 0;
        [SerializeField] bool ignoreInvincibility = false;

        //audio
        public Sound sfx;


        public override void AbilityPressed() {
            Shoot();
            
            isHolding = true;
        }

        public override void AbilityReleased() {
            isHolding = false;
        }
        public override void OnAbilityUnequipped() {
            isHolding = false;
        }

        public override void ModifyDamage(float delta)
        {
            projectileDamage += delta;
        }

        private void Update() {
            if (_cooldownTimer > 0) {
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

            PlaySound();

            Projectile projectile = Instantiate(projectilePrefab, transform.position + transform.TransformDirection(spawnOffset), Quaternion.identity);
            projectile.dmg = projectileDamage;
            if (ignoreInvincibility)
            {
                projectile.IgnoreInvincibility();
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = _player.transform.forward * projectileSpeed;
            
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