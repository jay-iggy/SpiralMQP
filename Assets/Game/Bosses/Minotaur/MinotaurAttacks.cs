using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Interfaces;

namespace Game.Scripts
{
    public class MinotaurAttacks : MonoBehaviour, ICanAttack
    {
        private const int CHARGE_ATTACK = 0;
        private const int FLAIL_ATTACK = 1;
        private const int SEEKING_CHARGE_ATTACK = 2;
        
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        [SerializeField] MinotaurFlail flail;
        private GameObject player;
        private Boss boss;

        public GameObject triangle;

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

            if(curAttack == SEEKING_CHARGE_ATTACK)
            {
                flail.StopTrailing();
            }

            curAttack = index;
            switch (curAttack)
            {
                case CHARGE_ATTACK:
                    return Charge();
                case FLAIL_ATTACK:
                    return FlailSmash();
                case SEEKING_CHARGE_ATTACK:
                    return SeekingCharge();
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
            turnDelta = .7f;
            flail.StartTrailing();
            timer.Set(4.5f, 2);
            return 5;
        }


        public void OnTimerEnd(int data)
        {

            switch (data)
            {
                case CHARGE_ATTACK:
                    charging = true;
                    break;
                case FLAIL_ATTACK:
                    //launch
                    flail.Launch();
                    break;
                case SEEKING_CHARGE_ATTACK:
                    flail.StopTrailing();
                    turnDelta = 2;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!lockRotation)
            {
                rotateTowardsPlayer();
                if (curAttack == CHARGE_ATTACK && facingPlayer) //charge
                {
                    //charge audio
                    chargeVelocity = player.transform.position - transform.position;
                    chargeVelocity.Normalize();
                    chargeVelocity *= chargeSpeed;
                    lockRotation = true;
                    timer.Set(.25f, 0);
                }
                else if(curAttack == SEEKING_CHARGE_ATTACK) //seeking charge
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
                if(curAttack == CHARGE_ATTACK) //charge
                {
                    charging = false;
                    Vector3 collisionPoint = other.ClosestPoint(transform.position);
                    WallHitBullets(collisionPoint);
                    Vector3 bounceVelocity = transform.position - collisionPoint;
                    bounceVelocity.Normalize();
                    GetComponent<MovementComponent>().AddExternalVelocity(bounceVelocity*10);
                    ScreenShake.instance.StartShake(.2f, .5f);
                    Charge();
                }
                if(curAttack == SEEKING_CHARGE_ATTACK) //seeking charge
                {
                    Vector3 collisionPoint = other.ClosestPoint(transform.position);
                    Vector3 bounceVelocity = transform.position - collisionPoint;
                    bounceVelocity.Normalize();
                    GetComponent<MovementComponent>().AddExternalVelocity(bounceVelocity * 10);
                    ScreenShake.instance.StartShake(.2f, .3f);
                    if(other.gameObject.name == "H")
                    {
                        Bounce(false);
                    }
                    else if(other.gameObject.name == "V")
                    {
                        Bounce(true);
                    }                  
                }
            }
            else if(other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<HealthComponent>().GetHit(1);
                if(curAttack == SEEKING_CHARGE_ATTACK)
                {
                    //bump into play
                    float xDif = Mathf.Abs(other.transform.position.x - transform.position.x);
                    float yDif = Mathf.Abs(other.transform.position.y - transform.position.y);
                    if (yDif > xDif)
                    {
                        Bounce(false);
                    }
                    else
                    {
                        Bounce(true);
                    }
                }
            }
        }

        private void Bounce(bool vertical)
        {
            float angle = triangle.transform.localEulerAngles.z;
            if (!vertical)
            {
                angle += 180;
                if (angle >= 360) angle -= 360;
            }

            angle = -angle + 360;
            triangle.transform.localEulerAngles = new Vector3(0, 0, angle);
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
            if (curAngle < 0) curAngle += 360;
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
            forwardVelocity *= chargeSpeed*.8f;
            transform.position += forwardVelocity;
        }
    }

}
