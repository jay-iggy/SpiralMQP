using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BasiliskAttacks : MonoBehaviour, ICanAttack {
        private const int BITE_ATTACK = 0;
        private const int TONGUE_ATTACK = 1;
        private const int CHASE_PLAYER = 2;
        private const int BACK_AWAY = 3;

        [SerializeField] Transform basiliskBody;
        [SerializeField] Transform tonguePoint;
        [SerializeField] Transform jawHinge;

        [SerializeField] GameObject biteHitbox;
        [SerializeField] Timer timer;
        private GameObject player;

        private GameObject curBite;

        private LineRenderer tongue;
        private Vector3 tongueEnd;
        private Vector3 tongueTarget;
        private float percentToTarget = 0; // [0,1]
        private float percentIncrement = 0.05f;
        private bool tongueStuck = false;
        private bool retractingTongue = false;
        private bool completedTongueAttack = false;

        private bool biteComplete = false;
        private bool mouthOpen = false;

        private Vector3 playerPetrifyPos;

        private Vector3 center = new Vector3(0, 2, 0);
        public float speed;

        private int curAttack = -1;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            tongue = this.GetComponent<LineRenderer>();
        }

        public int GetAttackCount() { return 4; }

        public float Attack(int index) {
            curAttack = index;

            switch (curAttack) {
                case BITE_ATTACK:
                    if (Vector3.Distance(basiliskBody.position, player.transform.position) > 10)
                        return Tongue();

                    return Bite();

                case TONGUE_ATTACK:
                    if (Vector3.Distance(basiliskBody.position, player.transform.position) < 2)
                        return Bite();

                    return Tongue();

                case CHASE_PLAYER:
                    return MoveTowardsTarget(player.transform.position);

                case BACK_AWAY:
                    return MoveAwayFromTarget(player.transform.position);
            }

            return 0;
        }

        private float Bite() {
            jawHinge.transform.rotation = Quaternion.Slerp(jawHinge.transform.rotation, Quaternion.Euler(-90f, jawHinge.transform.rotation.eulerAngles.y, jawHinge.transform.rotation.eulerAngles.z), 0.03f);
            MoveTowardsTarget(player.transform.position, 0.2f);
            biteComplete = false;
            return -1;
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

        public void OnTimerEnd(int data) {
            switch (data) {
                case BITE_ATTACK:
                    Bite();
                    break;
                case TONGUE_ATTACK:
                    Tongue();
                    break;
                case CHASE_PLAYER:
                    MoveTowardsTarget(player.transform.position);
                    break;
                case BACK_AWAY:
                    MoveAwayFromTarget(player.transform.position);
                    break;
            }
        }

        private float MoveTowardsTarget(Vector3 target)
        {
            Vector3 flatTarget = new Vector3(target.x, basiliskBody.position.y, target.z);
            basiliskBody.position = Vector3.MoveTowards(basiliskBody.position, flatTarget, speed);
            return 1;
        }
        private float MoveAwayFromTarget(Vector3 target)
        {
            Vector3 flatTarget = new Vector3(target.x, basiliskBody.position.y, target.z);
            Vector3 directionAway = basiliskBody.position - flatTarget; // Calculate direction away from the target
            basiliskBody.position = Vector3.MoveTowards(basiliskBody.position, basiliskBody.position + directionAway, speed);
            return 1;
        }

        private void MoveTowardsTarget(Vector3 target, float rate)
        {
            Vector3 flatTarget = new Vector3(target.x, basiliskBody.position.y, target.z);
            basiliskBody.position = Vector3.MoveTowards(basiliskBody.position, flatTarget, rate);
        }

        private void RotTowardsTarget(Vector3 target)
        {
            Vector3 flatTarget = new Vector3(target.x, basiliskBody.position.y, target.z);
            Vector3 directionToTarget = (flatTarget - basiliskBody.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            basiliskBody.rotation = Quaternion.Slerp(basiliskBody.rotation, lookRotation, Time.deltaTime * 10); // Smooth rotation
        }

        private void Update()
        {
            RotTowardsTarget(player.transform.position);
        }

        private void FixedUpdate() {
            switch (curAttack) {
                case BITE_ATTACK:
                    if (biteComplete)
                        break;

                    if (!mouthOpen)
                    {
                        MoveTowardsTarget(player.transform.position, 0.09f);

                        Quaternion targetRotation = Quaternion.Euler(-90f, jawHinge.transform.rotation.eulerAngles.y, jawHinge.transform.rotation.eulerAngles.z);
                        jawHinge.transform.rotation = Quaternion.Slerp(jawHinge.transform.rotation, targetRotation, 0.05f);

                        float currentX = jawHinge.transform.eulerAngles.x;
                        if (currentX >= 260f && currentX <= 280f) // Recognizing 270° as fully open
                            mouthOpen = true;
                    }
                    else
                    {
                        MoveTowardsTarget(player.transform.position, 0.05f);

                        Quaternion targetRotation = Quaternion.Euler(357f, jawHinge.transform.rotation.eulerAngles.y, jawHinge.transform.rotation.eulerAngles.z);
                        jawHinge.transform.rotation = Quaternion.Slerp(jawHinge.transform.rotation, targetRotation, 0.1f);
                        if(biteHitbox!=null) {
                            biteHitbox.SetActive(true);
                        }

                        float currentX = jawHinge.transform.eulerAngles.x;
                        if (currentX >= 345f || currentX < 10f) // Properly detects closing back to ~357
                        {
                            mouthOpen = false;
                            biteHitbox.SetActive(false);
                            biteComplete = true;
                            jawHinge.transform.rotation = Quaternion.Euler(357f, jawHinge.transform.rotation.eulerAngles.y, jawHinge.transform.rotation.eulerAngles.z); // Explicit reset
                            this.GetComponent<Boss>().DoneWithAttack();
                            return;
                        }
                    }

                    break;
                case TONGUE_ATTACK:
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
                        tongueEnd = Vector3.Lerp(tonguePoint.position, tongueTarget, percentToTarget);
                        RenderTongue(tonguePoint.position, tongueEnd);

                        if (tongueStuck)
                            player.transform.position = tongueEnd;
                    }
                    else // extend
                    {
                        tongueEnd = Vector3.Lerp(tonguePoint.position, tongueTarget, percentToTarget);
                        percentToTarget += percentIncrement;
                        RenderTongue(tonguePoint.position, tongueEnd);
                    }

                    break;
                case CHASE_PLAYER:
                    MoveTowardsTarget(player.transform.position);
                    break;
                case BACK_AWAY:
                    MoveAwayFromTarget(player.transform.position);
                    break;
            }
        }
    }
}
