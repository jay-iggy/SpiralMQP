using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class MachineAttack : MonoBehaviour, IHaveOneAttack
    {
        private GameObject bullet;
        public void SetBullet(GameObject b) {  bullet = b; }

        private Timer timer;
        int bulletAngle = 0;

        void Start()
        {
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
        }

        public float Attack()
        {
            bulletAngle = Random.Range(0, 2) * 15; //0 or 15
            GameObject[] bullets = MakeBulletCircle();
            BulletPatterns.MoveTowards(bullets, transform.position, -2);
            timer.Set(1, 0);

            return 1;
        }

        public void StopAttacking()
        {

        }

        private GameObject[] MakeBulletCircle()
        {
            GameObject[] bullets = new GameObject[12];
            for (int i = 0; i < 12; i++)
            {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, .75f, bulletAngle);

            return bullets;
        }

        public void OnTimerEnd(int data)
        {
            GameObject[] bullets = MakeBulletCircle();
            BulletPatterns.MoveTowards(bullets, transform.position, -12);
        }
    }
}
