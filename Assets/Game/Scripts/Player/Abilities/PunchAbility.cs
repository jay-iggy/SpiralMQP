﻿using System;
using System.Collections;
using Game.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class PunchAbility : Ability {
        
        // note: a better solution would be to play an animation that has the collider enabled for the duration of the punch
        
        
        [Header("Melee")]
        [SerializeField] GameObject fist;
        [SerializeField] float punchCooldown = .25f;
        [SerializeField] float punchDuration = .5f;
        private float _punchTimer = 0;
        public float dmg = 1;
        [SerializeField] private Collider magnetismTrigger;
        
        private void Start() {
            if(fist.TryGetComponent(out Hitbox hitbox)) {
                BindHitbox(hitbox);
            }
            
            punchCooldown /= CustomStatsManager.instance.customStats.playerAttackSpeed;
            punchDuration *= CustomStatsManager.instance.customStats.playerAttackSpeed;
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
                target.GetComponent<EnemyMovement>().AddExternalVelocity(direction * 5);
            }
        }

        
        public override void AbilityPressed() {
            if (_punchTimer > 0) {
                return;
            }
            fist.SetActive(true);
            _punchTimer = punchCooldown + punchDuration;
            magnetismTrigger.enabled = true;
            
            
            StartCoroutine(ResetPunchTimer());
        }

        public override void AbilityReleased() {
            // nothing to do here
        }
        
        private IEnumerator ResetPunchTimer() {
            // could just do a yield return new WaitForSeconds
            // though the other code is set up to support this way
            
            while (_punchTimer > 0) {
                _punchTimer -= Time.deltaTime;
                yield return null;
            }
            fist.SetActive(false);
            magnetismTrigger.enabled = false;
        }
        
        
        void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag(TagManager.Enemy)) { // TODO: make this based off hitbox tagsToHit
                Vector3 direction = other.transform.position - transform.position;
                direction.y = 0;
                _player.AddPersonalForce(direction * 1);
            }
        }
    }
}