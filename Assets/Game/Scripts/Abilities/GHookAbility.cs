using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player.Abilities
{
    public class GHookAbility : Ability
    {
        private bool canHook = true;
        private float hookCooldown = 3;

        private bool shooting = false;
        private bool attatched = false;

        private Vector3 grapplePoint;
        private Vector3 grappleEnd;
        public LayerMask whatIsGrappleable;
        public Transform player;
        private float maxDistance = 100f;
        private Vector3 lrOffset = Vector3.up * 0.8f;

        private float percentToTarget = 0; // [0,1]
        private float extendIncrement = 0.1f;
        private float retractIncrement = 0.01f;

        private LineRenderer lr;

        public Sound shootSfx;
        public Sound reelSfx;

        private void Start()
        {
            player = _player.transform;
            lr = GetComponent<LineRenderer>();
        }

        public override void AbilityPressed()
        {
            if (!canHook) return;

            Vector2 aim = _player._cumulativeLookInput;
            Vector3 hookDirection = aim.x * Vector3.right + aim.y * Vector3.forward;

            RaycastHit hit;
            if (Physics.Raycast(player.position, hookDirection, out hit, maxDistance, whatIsGrappleable))
            {
                shooting = true;
                grapplePoint = hit.point;
                shootSfx.PlaySound();
            }
        }

        private void FixedUpdate()
        {
            HandleGrapplingHook();
        }

        IEnumerator WaitForHookCooldown()
        {
            canHook = false;
            yield return new WaitForSeconds(hookCooldown);
            canHook = true;
        }

        public override void AbilityReleased()
        {
            // add knockback to the player in direction of ghook
            Vector3 direction = (grapplePoint - grappleEnd).normalized;
            this.transform.parent.parent.GetComponent<MovementComponent>().AddExternalVelocity(direction * 5);

            shooting = false;
            attatched = false;
            grapplePoint = Vector3.zero;
            grappleEnd = Vector3.zero;
            percentToTarget = 0;
            lr.positionCount = 0;
            // StartCoroutine(WaitForHookCooldown());
        }

        private void HandleGrapplingHook()
        {
            if (shooting) // extend
            {
                grappleEnd = Vector3.Lerp(this.transform.position, grapplePoint, percentToTarget);
                percentToTarget += extendIncrement;
                lr.positionCount = 2;
                lr.SetPosition(0, this.transform.position + lrOffset);
                lr.SetPosition(1, grappleEnd);

                if (percentToTarget >= 1)
                {
                    shooting = false;
                    attatched = true;
                    percentToTarget = 0;
                    reelSfx.PlaySound();
                }
            }

            if (attatched) // retract
            {
                percentToTarget += retractIncrement;
                player.transform.position = Vector3.Lerp(this.transform.position, grapplePoint, percentToTarget);
                lr.positionCount = 2;
                lr.SetPosition(0, this.transform.position + lrOffset);
                lr.SetPosition(1, grapplePoint);

                if (percentToTarget >= 1) // end when fully retracted
                {
                    lr.positionCount = 0;
                    percentToTarget = 0;
                    attatched = false;
                }
            }
        }
    }
}