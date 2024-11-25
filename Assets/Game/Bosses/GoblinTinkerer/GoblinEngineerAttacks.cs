using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class GoblinEngineerAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] GameObject bearTrap;
        [SerializeField] Timer timer;
        private GameObject player;

        private GameObject curBearTrap;

        private LineRenderer grapplingHook;
        private Vector3 gHookEnd;
        private Vector3 gHookTarget;
        private float percentToTarget = 0; // [0,1]
        private float percentIncrement = 0.05f;
        private Vector3 gHookOffset = new Vector3(0, 1, 0);
        private bool gHookClasped = false;
        private bool retractingGHook = false;
        private bool completedGHookAttack = false;

        private Vector3 playerPetrifyPos;

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;

        private int curAttack = -1;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            grapplingHook = this.GetComponent<LineRenderer>();
        }

        public int GetAttackCount() { return 5; }

        public float Attack(int index) {
            curAttack = index;
            switch (index) {
                case 0:
                    return BearTrap();
                case 1:
                    return GrapplingHook();
                case 2:
                    return SniperMusket();
                case 3:
                    return BallBearing();
                case 4:
                    if (GetComponent<HealthComponent>().health < 50)
                        return RocketBoots();
                    else
                        return BearTrap();
            }

            return 0;
        }

        private float BearTrap() {
            curBearTrap = Instantiate(bearTrap);
            curBearTrap.transform.position = this.transform.position + gHookOffset;
            timer.Set(2, 0);
            return 1;
        }

        private float GrapplingHook() {
            gHookTarget = player.transform.position;
            completedGHookAttack = false;
            return -1;
        }
        public void RenderGHook(Vector3 start, Vector3 end)
        {
            grapplingHook.positionCount = 2;
            Vector3[] points = { start, end };
            grapplingHook.SetPositions(points);
        }
        public void UnrenderGHook()
        {
            grapplingHook.positionCount = 0;
        }

        private float SniperMusket()
        {

            return 1;
        }

        private float GoToCenter() {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.Set(1, 2);
            return 2.25f;
        }

        private float BallBearing()
        {
            return 0;
        }

        private float RocketBoots()
        {
            return 0;
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case 0:
                    BearTrap();
                    break;
                case 1:
                    GrapplingHook();
                    break;
                case 2:
                    SniperMusket();
                    break;
            }
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
                case 0:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);

                    break;
                case 1:
                    if (completedGHookAttack)
                        break;

                    if (!gHookClasped && Vector3.Distance(player.transform.position, gHookEnd) < 1) // check if hit player 
                        gHookClasped = true;
                    if (percentToTarget >= 1) // check maxedLength
                        retractingGHook = true;

                    if (retractingGHook || gHookClasped) // retract
                    {
                        if (percentToTarget < 0) // end when fully retracted
                        {
                            UnrenderGHook();
                            percentToTarget = 0;
                            retractingGHook = false;
                            gHookClasped = false;
                            completedGHookAttack = true;
                            this.GetComponent<Boss>().DoneWithAttack();
                            return;
                        }

                        percentToTarget -= percentIncrement;
                        gHookEnd = Vector3.Lerp(this.transform.position + gHookOffset, gHookTarget, percentToTarget);
                        RenderGHook(this.transform.position + gHookOffset, gHookEnd);

                        if (gHookClasped)
                            player.transform.position = gHookEnd;
                    }
                    else // extend
                    {
                        gHookEnd = Vector3.Lerp(this.transform.position + gHookOffset, gHookTarget, percentToTarget);
                        percentToTarget += percentIncrement;
                        RenderGHook(this.transform.position + gHookOffset, gHookEnd);
                    }

                    break;
                case 2:
                    
                    break;
                case 3:

                    break;
                case 4:

                    break;
            }

            //respond to punch

        }
    }

}
