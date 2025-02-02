using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player.Abilities {
    public class DashAbility : Ability {
        // TODO: disable player movement during dash and for a short time after dashing

        //audio
        [SerializeField] bool invincibleDash = false;
        [SerializeField] bool damagingDash = false;

        public Sound sfx;

        [SerializeField] private float dashForce = 10f;
        private bool canDash = true;
        [SerializeField] private float dashCooldown = 1f;
        private float dashDuration = .25f;
        public override void AbilityPressed() {
            if(!canDash) return;

            
            
            Vector2 movementInput = _player.GetMovementInput();
            Vector3 dashDirection = movementInput.x * Vector3.right + movementInput.y * Vector3.forward;
            _player.movementComponent.AddPersonalVelocity(dashForce * dashDirection);
            StartCoroutine(WaitForDashCooldown());
            if (invincibleDash)
            {
                StartCoroutine(WaitForDashDuration());
            }

            PlaySound();
        }
        IEnumerator WaitForDashDuration()
        {
            BoxCollider bc = null;
            if (damagingDash)
            {
                bc = GetComponent<BoxCollider>();
                bc.enabled = true;
            }
            HealthComponent hc = _player.GetComponent<HealthComponent>();
            Physics.IgnoreLayerCollision(7, 10, true); //7=player, 10=enemy
            hc.invincible = true;
            yield return new WaitForSeconds(dashDuration);
            if (damagingDash)
            {
                bc.enabled = false;
            }
            hc.invincible = false;
            Physics.IgnoreLayerCollision(7, 10, false); //7=player, 10=enemy
        }
        IEnumerator WaitForDashCooldown() {
            canDash = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        public override void AbilityReleased() {
            // nothing to do here
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}