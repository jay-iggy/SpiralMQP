using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public class RatAttacks : MonoBehaviour, ICanAttack {
        private const int BIG_BULLET_ATTACK = 0;
        private const int SIX_BULLET_ATTACK = 1;
        private const int SHOOT_CIRCLE_BULLET_ATTACK = 2;
        private const int UPDATE_CIRCLE_BULLET_ATTACK = 3;
        
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

        [FormerlySerializedAs("gunPosition")] [SerializeField] private Transform gunPoint;
        private Vector3 bulletCircleCenter;

        [SerializeField] private UnityEvent onShootFX;
        [SerializeField] private Sound shootSfx;
        

        //audio
        public AudioManager AudioCON;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            //gun = transform.position + Vector3.left;
            onShootFX.AddListener(PlaySound);
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index) {
            curAttack = index;
            switch (curAttack) {
                case BIG_BULLET_ATTACK:
                    AudioCON.PlayShoot();
                    return ShootBigBullet();
                case SIX_BULLET_ATTACK:
                    AudioCON.PlayShoot();
                    shotsInChamber = 6;
                    return ShootSixBullets();
                case SHOOT_CIRCLE_BULLET_ATTACK:
                    AudioCON.PlayShoot();
                    return GoToCenter();
            }

            return 0;
        }

        private float ShootBigBullet() {
            setGunPoint();
            bigBullet = Instantiate(bullet, gunPoint.position, Quaternion.identity);
            bigBullet.transform.localScale = new Vector3(.5f, .5f, .5f);
            timer.Set(1, 0);
            return 1;
        }

        private float ShootSixBullets() {
            if (shotsInChamber > 0) {
                setGunPoint();
                bulletInChamber = Instantiate(bullet, gunPoint.position, Quaternion.identity);
                Projectile p = bulletInChamber.GetComponent<Projectile>();
                p.TargetPlayer(8);
                p.destroyedByWall = true;
                onShootFX.Invoke();
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
            curAttack = UPDATE_CIRCLE_BULLET_ATTACK;
            for(int i = 0; i < 12; i++) {
                bullets[i] = Instantiate(bullet);
            }
            bulletCircleCenter = transform.position;
            BulletPatterns.CreateCircle(bullets, bulletCircleCenter, 1);
            foreach (GameObject b in bullets) {
                b.transform.position = new Vector3(b.transform.position.x, gunPoint.position.y, b.transform.position.z);
            }
            timer.Set(.25f, 3);
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case BIG_BULLET_ATTACK:
                    if(bigBullet != null) {
                        bigBullet.GetComponent<Projectile>().TargetPlayer(5);
                        bigBullet = null;
                        ScreenShake.instance.StartShake(.2f, .3f);
                        onShootFX.Invoke();
                    }
                    curAttack = -1;
                    break;
                case SIX_BULLET_ATTACK:
                    ShootSixBullets();
                    break;
                case SHOOT_CIRCLE_BULLET_ATTACK:
                    ShootCirclePattern();
                    break;
                case UPDATE_CIRCLE_BULLET_ATTACK:
                    BulletPatterns.MoveTowards(bullets, bulletCircleCenter, -8);
                    foreach (GameObject b in bullets) {
                        if (b != null) {
                            b.GetComponent<Projectile>().destroyedByWall = true;
                        }
                    }
                    bullets = new GameObject[12];
                    break;
            }
        }

        private void setGunPoint()
        {
            /*if (player.transform.position.x > transform.position.x)
            {
                gun = transform.position + Vector3.right;
            }
            else
            {
                gun = transform.position + Vector3.left;
            }*/
        }

        private void Update() {
            Vector3 targetDir = player.transform.position - transform.position;
            float step = 3 * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        
        private void FixedUpdate() {
            switch (curAttack) {
                case BIG_BULLET_ATTACK:
                    float s = .04f;
                    if (bigBullet != null) {
                        bigBullet.transform.localScale += new Vector3(s, s, s);
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -.02f);
                        bigBullet.transform.position = gunPoint.position;
                    }
                    else {
                        // this happens when the player walks into the bullet before it is fired
                        curAttack = -1;
                    }
                    break;
                case SIX_BULLET_ATTACK:
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .025f);
                    break;
                case SHOOT_CIRCLE_BULLET_ATTACK:
                    // move to center before shooting
                    transform.position = Vector3.MoveTowards(transform.position, center, speed);
                    break;
            }

        }

        private void PlaySound() {
            shootSfx.PlaySound();
        }
    }

}
