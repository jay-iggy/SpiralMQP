using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BasiliskAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] GameObject biteHitbox;
        [SerializeField] Timer timer;
        private GameObject player;

        private GameObject curBite;

        private LineRenderer tongue;
        private Vector3 tongueEnd;
        private Vector3 tongueTarget;
        private float percentToTarget = 0; // [0,1]
        private float percentIncrement = 0.05f;
        private Vector3 mouthOffset = new Vector3(2, 1, 0);
        private bool tongueStuck = false;
        private bool retractingTongue = false;
        private bool completedTongueAttack = false;

        private Vector3 playerPetrifyPos;

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;

        private int curAttack = -1;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            tongue = this.GetComponent<LineRenderer>();
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index) {
            curAttack = index;
            switch (index) {
                case 0:
                    return Bite();
                case 1:
                    return Tongue();
                case 2:
                    return Petrify();
            }

            return 0;
        }

        private float Bite() {
            curBite = Instantiate(biteHitbox);
            curBite.transform.position = this.transform.position + mouthOffset;
            timer.Set(2, 0);
            return 1;
        }

        private float Tongue() {
            tongueTarget = player.transform.position;
            completedTongueAttack = false;
            return -1;
        }
        public void RenderTongue(Vector3 start, Vector3 end)
        {
            tongue.positionCount = 2;
            Vector3[] points = { start, end };
            tongue.SetPositions(points);
        }
        public void UnrenderTongue()
        {
            tongue.positionCount = 0;
        }

        private float Petrify()
        {
            playerPetrifyPos = player.transform.position;
            return 1;
        }

        private float GoToCenter() {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.Set(1, 2);
            return 2.25f;
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case 0:
                    Bite();
                    break;
                case 1:
                    Tongue();
                    break;
                case 2:
                    Petrify();
                    break;
            }
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
                case 0:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);

                    break;
                case 1:
                    if (completedTongueAttack)
                        break;

                    if (!tongueStuck && Vector3.Distance(player.transform.position, tongueEnd) < 1) // check if hit player 
                        tongueStuck = true;
                    if (percentToTarget >= 1) // check maxedLength
                        retractingTongue = true;

                    if (retractingTongue || tongueStuck) // retract
                    {
                        if (percentToTarget < 0) // end when fully retracted
                        {
                            UnrenderTongue();
                            percentToTarget = 0;
                            retractingTongue = false;
                            tongueStuck = false;
                            completedTongueAttack = true;
                            this.GetComponent<Boss>().DoneWithAttack();
                            return;
                        }

                        percentToTarget -= percentIncrement;
                        tongueEnd = Vector3.Lerp(this.transform.position + mouthOffset, tongueTarget, percentToTarget);
                        RenderTongue(this.transform.position + mouthOffset, tongueEnd);

                        if (tongueStuck)
                            player.transform.position = tongueEnd;
                    }
                    else // extend
                    {
                        tongueEnd = Vector3.Lerp(this.transform.position + mouthOffset, tongueTarget, percentToTarget);
                        percentToTarget += percentIncrement;
                        RenderTongue(this.transform.position + mouthOffset, tongueEnd);
                    }

                    break;
                case 2:
                    transform.position = Vector3.MoveTowards(transform.position, center, speed);
                    player.transform.position = playerPetrifyPos;
                    break;
            }

            //respond to punch

        }
    }

}
