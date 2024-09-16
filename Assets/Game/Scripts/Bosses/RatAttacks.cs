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
        private GameObject player;

        private GameObject bigBullet;

        private int shotsInChamber;
        private GameObject bulletInChamber;

        private Vector3 center = new Vector3(0, 2, -10);
        private float speed;
        GameObject[] bullets = new GameObject[12];

        private int curAttack = -1;

        private Vector3 gun;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            timer.timeUp.AddListener(onTimerEnd);
            gun = transform.position + Vector3.left;
        }

        public int numAttacks() { return 3; }

        public float Attack(int index)
        {
            curAttack = index;
            switch (index)
            {
                case 0:
                    return bigShot();
                case 1:
                    shotsInChamber = 6;
                    return sixShot();
                case 2:
                    return goToCenter();
            }

            return 0;
        }

        private float bigShot()
        {
            bigBullet = Instantiate(bullet, gun, Quaternion.identity);
            bigBullet.transform.localScale = new Vector3(.5f, .5f, .5f);
            timer.set(1, 0);
            return 1;
        }

        private float sixShot()
        {
            if (shotsInChamber > 0)
            {
                bulletInChamber = Instantiate(bullet, gun, Quaternion.identity);
                bulletInChamber.GetComponent<Projectile>().targetPlayer(8);
            }

            if (shotsInChamber >= 0)
            {
                shotsInChamber--;
                timer.set(.5f, 1);
            }
            else
            {
                bulletInChamber = null;
            }
            return 3.5f;
        }

        private float goToCenter()
        {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.set(1, 2);
            return 2.5f;
        }

        private void circleShot()
        {
            curAttack = 3;
            for(int i = 0; i<12; i++)
            {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.circle(bullets, transform.position, 2, 8);
            timer.set(.5f, 3);
        }

        public void onTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    bigBullet.GetComponent<Projectile>().targetPlayer(5);
                    bigBullet = null;
                    curAttack = -1;
                    break;
                case 1:
                    sixShot();
                    break;
                case 2:
                    circleShot();
                    break;
                case 3:
                    BulletPatterns.moveTowards(bullets, center, -8);
                    bullets = new GameObject[12];
                    break;
            }
        }
        
        private void FixedUpdate()
        {
            if(player.transform.position.x > transform.position.x)
            {
                gun = transform.position + Vector3.right;
            }
            else
            {
                gun = transform.position + Vector3.left;
            }

            switch (curAttack)
            {
                case 0:
                    float s = .04f;
                    bigBullet.transform.localScale += new Vector3(s, s, s);
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -.02f);
                    bigBullet.transform.position = gun;
                    break;
                case 1:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);
                    break;
                case 2:
                    transform.position = Vector3.MoveTowards(transform.position, center, speed);
                    break;
            }
        }
    }

}
