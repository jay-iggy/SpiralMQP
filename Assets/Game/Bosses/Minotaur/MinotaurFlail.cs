using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class MinotaurFlail : MonoBehaviour
    {
        private bool spinning = false;
        private bool shooting = false;
        private bool trailing = false;
        private float angularVelocity = 0;
        private GameObject player;
        [SerializeField] GameObject flailTip;
        float flailPosition;
        Timer timer;
        public MinotaurAttacks minotaur;

        private int launchStage = 0;
        private Vector3 launchTarget;
        private float releaseAtRotation;

        [SerializeField] GameObject bullet;
        private float shootTimer = 0;
        [SerializeField] float fireSpeed = .08f;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimeUp);
            flailPosition = flailTip.transform.localPosition.y;
        }

        public void StartSpinning()
        {
            fireSpeed = .08f;
            spinning = true;
            shooting = true;
        }

        public void StopSpinning()
        {
            spinning = false;
            shooting = false;
        }

        public void StartTrailing()
        {
            fireSpeed = .5f;
            trailing = true;
            shooting = true;
        }

        public void StopTrailing()
        {
            trailing = false;
            shooting = false;
        }

        public void Launch()
        {
            launchStage = 1;
            launchTarget = player.transform.position;
            float opposite = player.transform.position.x - transform.position.x;
            float adjacent = player.transform.position.z - transform.position.z;
            if(adjacent == 0)
            {
                if (opposite < 0) releaseAtRotation = 0;
                else releaseAtRotation = 180;
                return;
            }

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
            releaseAtRotation = angle - 90;
            if (releaseAtRotation < 0) releaseAtRotation += 360;
        }

        public void OnTimeUp(int data)
        {           
            switch (data)
            {
                case 0:
                    flailSmashBullets(15);
                    break;
                case 1:
                    flailSmashBullets(0);
                    break;
                case 2:
                    launchStage = 4;
                    return;
            }
            timer.Set(.3f, data + 1);
        }

        private void FixedUpdate()
        {
            if(launchStage == 1 && transform.localEulerAngles.z >= releaseAtRotation && transform.localEulerAngles.z < releaseAtRotation + angularVelocity)
            {
                //shoot out flail
                launchStage = 2;
                spinning = false;
                shooting = false;
                shootTimer = 0;
            }

            if(launchStage == 2)
            {
                flailTip.transform.localPosition += new Vector3(0, .2f, 0);
                if(Vector3.Distance(flailTip.transform.position, transform.position) >= Vector3.Distance(launchTarget, transform.position))
                {
                    launchStage = 3;
                    ScreenShake.instance.StartShake(.5f, 1);
                    //hits ground
                    flailSmashBullets(0);
                    timer.Set(.3f, 0);
                }
            }

            if(launchStage == 4)
            {
                flailTip.transform.localPosition -= new Vector3(0, .1f, 0);
                if(flailTip.transform.localPosition.y <= flailPosition + .1f)
                {
                    flailTip.transform.localPosition = new Vector3(0, flailPosition, 0);
                    launchStage = 0;
                    minotaur.FinishSmash();
                }
            }

            if (spinning && angularVelocity < 11)
            {
                angularVelocity += .5f;
            }
            if (!spinning && angularVelocity > 0)
            {
                angularVelocity -= .65f;
            }
            if (angularVelocity < 0)
            {
                angularVelocity = 0;
            }

            if (trailing)
            {
                float targetRotation = minotaur.triangle.transform.localEulerAngles.z + 180;
                if (targetRotation >= 360) targetRotation -= 360;

                float angleDifference = transform.localEulerAngles.z - targetRotation;
                if (Mathf.Abs(angleDifference) > 5f)
                {
                    if (angleDifference < 0)
                    {
                        angularVelocity = 4;
                    }
                    else
                    {
                        angularVelocity = -4;
                    }
                }
                else
                {
                    angularVelocity = 0;
                }
            }
            

            Quaternion deltaQ = new Quaternion();
            deltaQ.eulerAngles = new Vector3(0, 0, angularVelocity);
            transform.rotation *= deltaQ;
            if (transform.localEulerAngles.z >= 360)
            {
                transform.localEulerAngles -= new Vector3(0, 0, 360);
            }
            if (transform.eulerAngles.z < 0)
            {
                transform.localEulerAngles += new Vector3(0, 0, 360);
            }

            if (shooting)
            {
                
                shootTimer += Time.deltaTime;
                if(shootTimer >= fireSpeed) //shoot bullet
                {
                   /* if (trailing) //trail sound
                else // spin sound */
                                GameObject curBullet = Instantiate(bullet);
                    curBullet.transform.position = new Vector3(flailTip.transform.position.x, transform.position.y, flailTip.transform.position.z);
                    BulletPatterns.MoveTowards(curBullet, transform.position, -8);
                    shootTimer = 0;
                }
            }
        }

        private void flailSmashBullets(int angle)
        {
            GameObject[] bullets = new GameObject[12];
            for(int i = 0; i<12; i++)
            {
                bullets[i] = Instantiate(bullet);
            }

            BulletPatterns.CreateCircle(bullets, flailTip.transform.position, .1f, angle);
            BulletPatterns.MoveTowards(bullets, flailTip.transform.position, -8);
        }
    }
}
