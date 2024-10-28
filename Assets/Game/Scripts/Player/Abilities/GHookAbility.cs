using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player.Abilities {
    public class GHookAbility : Ability {
        public float hookForce;
        private bool canHook = true;
        private float hookCooldown = 3;
        private bool attatched = false;

        private Vector3 grapplePoint;
        public LayerMask whatIsGrappleable;
        public Transform player;
        private float maxDistance = 100f;

        private GHookSpring spring;
        private LineRenderer lr;
        private Vector3 currentGrapplePosition;
        public int quality;
        public float damper;
        public float strength;
        public float velocity;
        public float waveCount;
        public float waveHeight;
        public AnimationCurve affectCurve;

        private void Start()
        {
            player = _player.transform;
            lr = GetComponent<LineRenderer>();
            spring = new GHookSpring();
            spring.SetTarget(0);
        }

        public override void AbilityPressed() {
            if(!canHook) return;

            Vector2 aim = _player._cumulativeLookInput;
            Vector3 hookDirection = (aim.x * Vector3.right + aim.y * Vector3.forward);

            RaycastHit hit;
            if (Physics.Raycast(player.position, hookDirection, out hit, maxDistance, whatIsGrappleable))
            {
                Debug.DrawRay(player.position, hookDirection);
                attatched = true;
                grapplePoint = hit.point;
                Debug.Log("GRAPPLE PT: " + grapplePoint);
                DrawRope();
            }

            // _player.movementComponent.AddPersonalVelocity(hookForce * hookDirection);
        }

        private void Update()
        {
            if(attatched)
                DrawRope();
        }

        IEnumerator WaitForHookCooldown() {
            canHook = false;
            yield return new WaitForSeconds(hookCooldown);
            canHook = true;
        }

        public override void AbilityReleased() {
            attatched = false;
            lr.positionCount = 0;
            // StartCoroutine(WaitForHookCooldown());
        }

        public bool IsGrappling()
        {
            return attatched;
        }

        public Vector3 GetGrapplePoint()
        {
            return grapplePoint;
        }

        void DrawRope()
        {
            //If not grappling, don't draw rope
            if (!IsGrappling())
            {
                currentGrapplePosition = this.transform.position;
                spring.Reset();
                if (lr.positionCount > 0)
                    lr.positionCount = 0;
                return;
            }

            if (lr.positionCount == 0)
            {
                spring.SetVelocity(velocity);
                lr.positionCount = quality + 1;
                currentGrapplePosition = this.transform.position;
            }

            spring.SetDamper(damper);
            spring.SetStrength(strength);
            spring.UpdateSpring(Time.deltaTime);

            var grapplePoint = GetGrapplePoint();
            var startPos = this.transform.position;
            var up = Quaternion.LookRotation((grapplePoint - startPos).normalized) * Vector3.forward;

            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);
            Debug.Log(currentGrapplePosition);

            for (var i = 0; i < quality + 1; i++)
            {
                var delta = i / (float)quality;
                var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                             affectCurve.Evaluate(delta);

                lr.SetPosition(i, Vector3.Lerp(startPos, currentGrapplePosition, delta) + offset);
            }
        }
    }
}