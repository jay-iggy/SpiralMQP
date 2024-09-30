using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Interfaces;

namespace Game.Scripts
{
    public class MinotaurAttacks : MonoBehaviour, ICanAttack
    {
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;
        private Boss boss;

        [SerializeField] GameObject triangle;

        private int curAttack = -1;
        private bool facingPlayer = false;
        private bool lockRotation = false;
        private float turnDelta = 2;
       
        private int chargesToDo = -1;
        [SerializeField] int maxCharges = 5;
        bool charging = false;
        float chargeSpeed = .2f;
        Vector3 chargeVelocity;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
            boss = GetComponent<Boss>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
        }

        public int GetAttackCount() { return 2; }

        public float Attack(int index)
        {
            curAttack = index;
            switch (index)
            {
                case 0:
                    return Charge();
                case 1:
                    return FlailSmash();
                case 2:
                    return SeekingCharge();
                case 3:
                    return Horns();
            }
            return 0;
        }

        private float Charge()
        {
            lockRotation = false;
            if (chargesToDo == -1)
            {
                chargesToDo = Random.Range(0, maxCharges)+1;
            }
            else if (chargesToDo == 0)
            {
                chargesToDo = -1;
                curAttack = -1;
                boss.DoneWithAttack();
                return 0;
            }
            chargesToDo--;

            return -1;
        }

        private float FlailSmash()
        {
            transform.position = new Vector3(0, transform.position.y, 0);
            return 1;
        }

        private float SeekingCharge()
        {
            return 0;
        }

        private float Horns()
        {
            return 0;
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    charging = true;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!lockRotation)
            {
                rotateTowardsPlayer();
                if (curAttack == 0 && facingPlayer)
                {
                    chargeVelocity = player.transform.position - transform.position;
                    chargeVelocity.Normalize();
                    chargeVelocity *= chargeSpeed;
                    lockRotation = true;
                    timer.Set(.25f, 0);
                }
            }               
            if (charging)
            {
                transform.position += chargeVelocity;
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Wall")
            {
                if(curAttack == 0)
                {
                    charging = false;
                    Vector3 collisionPoint = other.ClosestPoint(transform.position);
                    WallHitBullets(collisionPoint);
                    Vector3 bounceVelocity = transform.position - collisionPoint;
                    bounceVelocity.Normalize();
                    GetComponent<EnemyMovement>().AddExternalVelocity(bounceVelocity*10);
                    ScreenShake.instance.StartShake(.2f, .5f);
                    Charge();
                }
            }
            else if(other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<HealthComponent>().GetHit(1);
            }
        }

        private void WallHitBullets(Vector3 center)
        {
            GameObject[] bullets = new GameObject[16];
            for (int i = 0; i < 16; i++)
            {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bullets, center, .1f);
            BulletPatterns.MoveTowards(bullets, center, -8);
        }

        private void rotateTowardsPlayer()
        {
            float opposite = player.transform.position.x - transform.position.x;
            float adjacent = player.transform.position.z - transform.position.z;
            if (adjacent == 0) adjacent = .01f; //divide by 0 protection

            float angle = Mathf.Atan(Mathf.Abs(opposite / adjacent));
            if (opposite > 0 && adjacent > 0)
            {
                angle = -angle;
            }
            else if (opposite > 0 && adjacent < 0)
            {
                angle += Mathf.PI;
            }
            else if (opposite < 0 && adjacent < 0)
            {
                angle = -angle;
                angle += Mathf.PI;
            }
            angle *= Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            float curAngle = triangle.transform.localEulerAngles.z;
            if (curAngle > angle - turnDelta && curAngle < angle + turnDelta)
            {
                facingPlayer = true;
                return;
            }

            float angleDistance = curAngle - angle;

            if (angleDistance > 180 || (angleDistance > -180 && angleDistance < 0)) //ccw
            {
                Quaternion deltaQ = new Quaternion();
                deltaQ.eulerAngles = new Vector3(0, 0, turnDelta);
                triangle.transform.rotation *= deltaQ;
            }
            else //cw
            {
                Quaternion deltaQ = new Quaternion();
                deltaQ.eulerAngles = new Vector3(0, 0, -turnDelta);
                triangle.transform.rotation *= deltaQ;
            }

            curAngle = triangle.transform.localEulerAngles.z;
            if (curAngle > angle - turnDelta && curAngle < angle + turnDelta)
            {
                facingPlayer = true;
            }
            else
            {
                facingPlayer = false;
            }
        }
    }

}
