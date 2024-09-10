using System.Collections;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class PunchAbility : Ability {
        
        [Header("Melee")]
        [SerializeField] GameObject fist;
        [SerializeField] float punchCooldown = .25f;
        [SerializeField] float punchDuration = .5f;
        private float _punchTimer = 0;
        
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