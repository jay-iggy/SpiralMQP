using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player.Abilities {
    public class DashAbility : Ability {
        // TODO: disable player movement during dash and for a short time after dashing
        
        [SerializeField] private float dashForce = 10f;
        private bool canDash = true;
        [SerializeField] private float dashCooldown = 1f;
        public override void AbilityPressed() {
            if(!canDash) return;
            // get player facing direction
            Vector2 movementInput = _player.GetMovementInput();
            Vector3 dashDirection = movementInput.x * Vector3.right + movementInput.y * Vector3.forward;
            
            _player.movementComponent.AddPersonalVelocity(dashForce * dashDirection);
            StartCoroutine(WaitForDashCooldown());
        }
        IEnumerator WaitForDashCooldown() {
            canDash = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        public override void AbilityReleased() {
            // nothing to do here
        }
    }
}