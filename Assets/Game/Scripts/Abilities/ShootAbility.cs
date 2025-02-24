using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Player.Abilities {
    public class ShootAbility : AttackAbility {
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;
        public float cooldown = 0.33f;
        public Projectile projectilePrefab;
        public bool isAutomatic = false;
        public bool canCrit = true;
        private bool isHolding = false;
        private float _cooldownEndTime = 0;
        [SerializeField] bool ignoreInvincibility = false;
        private static int _nextProjID = 0;
        [SerializeField] private Transform projectileSpawnPoint;

        //audio
        public Sound sfx;

        public override void OnAbilityEquipped() {
            _player.GetReticle().enabled = true;
        }

        private void Start()
        {
            baseDamage = projectileDamage;
            PlayerController pc = transform.parent.parent.GetComponent<PlayerController>();
            BindToPlayer(pc);
        }

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

        private void Update() {
            _player.GetReticle().color = CanShoot() ? Color.white : Color.gray;
            
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

            Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projectile.dmg = CalculateDamage();
            if (canCrit) {
                projectile.projID = _nextProjID++;
            }
            if (ignoreInvincibility)
            {
                projectile.IgnoreInvincibility();
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = _player.transform.forward * projectileSpeed;
            
            _cooldownEndTime = Time.time + cooldown;
        }
        
        private bool CanShoot() {
            return Time.time > _cooldownEndTime;
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}