using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Scripts.Player.Abilities {
    public class ShottgunAbility : AttackAbility {
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;
        public float cooldown = 0.33f;
        public Projectile projectilePrefab;
        public bool isAutomatic = false;
        private bool isHolding = false;
        private float _cooldownOverTimer = 0;
        private int _nextProjID = 0;
        [SerializeField] private Transform projectileSpawnPoint;

        public override void OnAbilityEquipped() {
            _player.GetReticle().enabled = true;
        }

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

            for (int i = 0; i < 6; i++)
            {
                Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
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
            _cooldownOverTimer = Time.time + cooldown;

        }
             
        private bool CanShoot() {
            return Time.time > _cooldownOverTimer;
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}