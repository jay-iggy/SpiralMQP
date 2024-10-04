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
        [SerializeField] MinotaurFlail flail;
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
            flail.minotaur = this;
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {
            if(curAttack == 2)
            {
                flail.StopSpinning();
            }

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
            turnDelta = 4;
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
                turnDelta = 2;
                return 0;
            }
            chargesToDo--;

            return -1;
        }

        private float FlailSmash()
        {
            flail.StartSpinning();
            timer.Set(2, 1);
            return -1;
        }

        public void FinishSmash()
        {
            boss.DoneWithAttack();
        }

        private float SeekingCharge()
        {
            turnDelta = .5f;
            flail.StartSpinning();
            timer.Set(4.5f, 2);
            return 5;
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
                case 1:
                    flail.Launch();
                    break;
                case 2:
                    flail.StopSpinning();
                    turnDelta = 2;
                    break;
                case -2:
                    turnDelta = .5f;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!lockRotation)
            {
                rotateTowardsPlayer();
                if (curAttack == 0 && facingPlayer) //charge
                {
                    chargeVelocity = player.transform.position - transform.position;
                    chargeVelocity.Normalize();
                    chargeVelocity *= chargeSpeed;
                    lockRotation = true;
                    timer.Set(.25f, 0);
                }
                else if(curAttack == 2) //seeking charge
                {
                    moveForward();
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
                if(curAttack == 0) //charge
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
                if(curAttack == 2) //seeking charge
                {
                    Vector3 collisionPoint = other.ClosestPoint(transform.position);
                    Vector3 bounceVelocity = transform.position - collisionPoint;
                    bounceVelocity.Normalize();
                    GetComponent<EnemyMovement>().AddExternalVelocity(bounceVelocity * 10);
                    ScreenShake.instance.StartShake(.2f, .3f);
                    turnDelta = 4;
                    timer.Set(.2f, -2);
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

        private void moveForward()
        {
            float angle = triangle.transform.localEulerAngles.z * Mathf.Deg2Rad;
            float sine = Mathf.Sin(angle);
            float cosine = Mathf.Cos(angle);
            Vector3 forwardVelocity = new Vector3(-sine, 0, cosine);
            forwardVelocity *= chargeSpeed * .35f;
            transform.position += forwardVelocity;
        }
    }

}
