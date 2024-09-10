﻿using System;
using System.Collections;
using Game.Scripts.Interfaces;
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

        private void Start() {
            if(fist.TryGetComponent(out Hitbox hitbox)) {
                BindHitbox(hitbox);
            }
        }
        void BindHitbox(Hitbox hitbox) {
            hitbox.onHit.AddListener(OnHitTarget);
        }
        private void OnHitTarget(ICanGetHit canGetHit) {
            canGetHit.Hit(dmg);
        }

        
        public override void AbilityPressed() {
            if (_punchTimer > 0) {
                return;
            }
            fist.SetActive(true);
            _punchTimer = punchCooldown + punchDuration;
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
        }
    }
}