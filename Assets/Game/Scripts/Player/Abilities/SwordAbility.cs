using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class SwordAbility : Ability {
        [Header("Melee")]
        [SerializeField] GameObject prefabSword;
        private GameObject sword;
        [SerializeField] Vector3 swordOffset;
        [SerializeField] float swordCooldown = .25f;
        [SerializeField] float swordDuration = .5f;
        private float _swordTimer = 0;
        public float dmg = 1;
        
        private void Start() {

            Quaternion startingRotation = prefabSword.transform.rotation;

            sword = Instantiate(prefabSword, this.transform.position + swordOffset, prefabSword.transform.rotation, this.transform);

            Vector2 direction = this.transform.parent.parent.GetComponent<PlayerController>()._cumulativeLookInput - (Vector2)this.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sword.transform.rotation = Quaternion.Euler(sword.transform.rotation.x, sword.transform.rotation.y, angle);

            if (sword.TryGetComponent(out Hitbox hitbox)) {
                BindHitbox(hitbox);
            }

            swordCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            swordCooldown *= CustomStatsManager.instance.customStats.playerAttackSpeed;
        }
        void BindHitbox(Hitbox hitbox) {
            hitbox.onHitTarget.AddListener(ProcessAttack);
        }
        private void ProcessAttack(ICanGetHit hurtbox) {
            hurtbox.GetHit(dmg);
            
            // knockback the target
            if(hurtbox is MonoBehaviour target) {
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                target.GetComponent<MovementComponent>().AddExternalVelocity(direction * 5);
            }
        }

        
        public override void AbilityPressed() {
            if (_swordTimer > 0) {
                return;
            }
            sword.transform.position = this.transform.position;
            sword.SetActive(true);
            _swordTimer = swordCooldown + swordDuration;
            
            
            StartCoroutine(ResetPunchTimer());
        }

        public override void AbilityReleased() {
            // nothing to do here
        }
        
        private IEnumerator ResetPunchTimer() {
            // could just do a yield return new WaitForSeconds
            // though the other code is set up to support this way
            
            while (_swordTimer > 0) {
                _swordTimer -= Time.deltaTime;
                yield return null;
            }
            sword.SetActive(false);
        }
        
        
        void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag(TagManager.Enemy)) { // TODO: make this based off hitbox tagsToHit
                Vector3 direction = other.transform.position - transform.position;
                direction.y = 0;
                _player.movementComponent.AddPersonalVelocity(direction * 1);
            }
        }
    }
}