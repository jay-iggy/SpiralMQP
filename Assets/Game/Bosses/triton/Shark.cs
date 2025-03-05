using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public enum SharkState
    {
        SUBMERGED,
        CHASING,
        LEAPING,
        DYING
    }

    public class Shark : MonoBehaviour
    {
        private BoxCollider coll;
        private HealthComponent hp;
        private GameObject player;
        public TurnHandler turnHandler;
        [Tooltip("submerged, surface, leap")]
        [SerializeField] float[] yLevels;
        public SharkState state = SharkState.SUBMERGED;
        private float forwardSpeed = .075f;
        private float verticalSpeed = 0;
        private float leapCooldown = 0;
        [SerializeField] private Sound warningSfx;
        [SerializeField] private Sound leapSfx;
        [SerializeField] private Sound biteSfx;

        void Start()
        {
            coll = GetComponent<BoxCollider>();
            hp = GetComponent<HealthComponent>();
            turnHandler = GetComponent<TurnHandler>();
            turnHandler.turnDelta = 1;
            turnHandler.viewConeAngle = 18;
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
        }

        public void Die()
        {
            Debug.Log("shark dead");
            hp.isAlive = true;
            state = SharkState.DYING;
            hp.Heal(15);
        }

        private void FixedUpdate()
        {
            if(leapCooldown > 0)
            {
                leapCooldown--;
            }

            switch (state)
            {
                case SharkState.DYING:
                    if (transform.position.y > yLevels[0])
                    {
                        transform.position += Vector3.down * .1f;
                    }
                    else
                    {
                        state = SharkState.SUBMERGED;
                    }
                    break;
                case SharkState.SUBMERGED:
                    if (transform.position.y > yLevels[0])
                    {
                        transform.position += Vector3.down * .02f;
                    }
                    if (forwardSpeed > 0)
                    {
                        forwardSpeed -= .0015f;
                    }
                    break;
                case SharkState.CHASING:
                    warningSfx.PlaySound();
                    if (transform.position.y < yLevels[1])
                    {
                        transform.position += Vector3.up * .02f;
                    }
                    if (forwardSpeed < .075f)
                    {
                        forwardSpeed += .0015f;
                    }
                    turnHandler.TurnTowards(player.transform.position);
                    turnHandler.MoveForward(forwardSpeed);
                    if (leapCooldown <= 0 && turnHandler.facingTarget && Mathf.Abs(Vector3.Distance(transform.position, player.transform.position)) < 10)
                    {
                        leapCooldown = 50;
                        state = SharkState.LEAPING;
                        //leap sound
                        leapSfx.PlaySound();
                        verticalSpeed = .2f;
                        forwardSpeed = .15f;
                        coll.enabled = true;
                    }
                    break;
                case SharkState.LEAPING:
                    transform.position += Vector3.up * verticalSpeed;
                    verticalSpeed -= .01f;
                    turnHandler.MoveForward(forwardSpeed);
                    if (transform.position.y <= yLevels[1])
                    {
                        transform.position = new Vector3(transform.position.x, yLevels[1], transform.position.z);
                        state = SharkState.CHASING;
                        forwardSpeed = .075f;
                        coll.enabled = false;
                    }
                    break;

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(TagManager.Player)) {
                other.gameObject.GetComponent<HealthComponent>().GetHit(1);
                biteSfx.PlaySound();
            }

        }
    }
}
