using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class RatAttacks : MonoBehaviour, ICanAttack
    {
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;

        private GameObject bigBullet = null;

        public int numAttacks() { return 3; }

        public float Attack(int index)
        {
            switch (index)
            {
                case 0:
                    return bigShot();
                case 1:
                    return sixShot();
                case 2:
                    return circleShot();
            }

            return 0;
        }

        private float bigShot()
        {
            bigBullet = Instantiate(bullet, transform.position+Vector3.left, Quaternion.identity);
            bigBullet.transform.localScale = new Vector3(.5f, .5f, .5f);
            timer.set(1, 0);
            timer.timeUp.AddListener(onTimerEnd);
            return 1;
        }

        private float sixShot()
        {
            transform.position = new Vector3(-2, 2, -10);
            return 2;
        }

        private float circleShot()
        {
            transform.position = new Vector3(3, 2, -10);
            return 2;
        }

        public void onTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    if (bigBullet == null) break;
                    bigBullet.GetComponent<Projectile>().targetPlayer(5);
                    bigBullet = null;
                    break;
            }
        }
        
        private void FixedUpdate()
        {
            if(bigBullet != null)
            {
                float s = .04f;
                bigBullet.transform.localScale += new Vector3(s, s, s);
            }
        }
    }

}
