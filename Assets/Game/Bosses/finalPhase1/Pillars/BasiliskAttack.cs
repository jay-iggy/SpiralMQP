using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BasiliskAttack : MonoBehaviour, IHaveOneAttack
    {
        private GameObject bullet;
        public void SetBullet(GameObject b) { bullet = b; }


        private LineRenderer tongue;
        private Vector3 tongueEnd;
        private Vector3 tongueTarget;
        private float percentToTarget = 0; // [0,1]
        private float percentIncrement = 0.05f;
        private Vector3 mouthOffset = new Vector3(0, 0, 0);
        private bool tongueStuck = false;
        private bool retractingTongue = false;
        private bool completedTongueAttack = true;
        private GameObject player;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
            tongue = GetComponent<LineRenderer>();
        }

        public float Attack()
        {
            Tongue();

            return 2;
        }

        private float Tongue()
        {
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

        public void StopAttacking()
        {

        }

        void FixedUpdate()
        {
            if (completedTongueAttack)
                return;


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
        }
    }
}
