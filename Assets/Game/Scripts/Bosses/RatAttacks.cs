using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class RatAttacks : MonoBehaviour, ICanAttack {
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;

        private GameObject bigBullet;

        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;
        GameObject[] bullets = new GameObject[12]; // why not just use a list?

        private int curAttack = -1;

        private Vector3 gun;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            gun = transform.position + Vector3.left;
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index) {
            curAttack = index;
            switch (index) {
                case 0:
                    return ShootBigBullet();
                case 1:
                    shotsInChamber = 6;
                    return ShootSixBullets();
                case 2:
                    return GoToCenter();
            }

            return 0;
        }

        private float ShootBigBullet() {
            setGunPoint();
            bigBullet = Instantiate(bullet, gun, Quaternion.identity);
            bigBullet.transform.localScale = new Vector3(.5f, .5f, .5f);
            timer.Set(1, 0);
            return 1;
        }

        private float ShootSixBullets() {
            if (shotsInChamber > 0) {
                setGunPoint();
                bulletInChamber = Instantiate(bullet, gun, Quaternion.identity);
                bulletInChamber.GetComponent<Projectile>().TargetPlayer(8);
            }

            if (shotsInChamber >= 0) {
                shotsInChamber--;
                timer.Set(.5f, 1);
            }
            else {
                bulletInChamber = null;
            }
            return 3.5f;
        }

        private float GoToCenter() {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.Set(1, 2);
            return 2.25f;
        }

        private void ShootCirclePattern() {
            curAttack = 3;
            for(int i = 0; i < 12; i++) {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, 1);
            timer.Set(.25f, 3);
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case 0:
                    bigBullet.GetComponent<Projectile>().TargetPlayer(5);
                    bigBullet = null;
                    curAttack = -1;
                    break;
                case 1:
                    ShootSixBullets();
                    break;
                case 2:
                    ShootCirclePattern();
                    break;
                case 3:
                    BulletPatterns.MoveTowards(bullets, transform.position, -8);
                    bullets = new GameObject[12];
                    break;
            }
        }

        private void setGunPoint()
        {
            if (player.transform.position.x > transform.position.x)
            {
                gun = transform.position + Vector3.right;
            }
            else
            {
                gun = transform.position + Vector3.left;
            }
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
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
