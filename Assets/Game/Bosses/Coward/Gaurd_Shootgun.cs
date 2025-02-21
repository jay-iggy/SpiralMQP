using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Gaurd_Shootgun : MonoBehaviour, ICanAttack
    {
       
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;
        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private Vector3 gun;
        public float waitDuration = 1f;



        //audio
        public AudioManager AudioCON;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            gun = transform.position + Vector3.left;
            
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {
            if (waitDuration <= 0)
            {
                return ShootBullets();
            }
            return 0;
        }

        private float ShootBullets()
        {
            
                int bulletCount = 5;
                float coneAngle = 30f;
                float bulletSpeed = 5f;


                GameObject[] bullets = new GameObject[bulletCount];
                Vector3 origin = transform.position;
                Vector3 baseDirection = (player.transform.position - origin).normalized;


                for (int i = 0; i < bulletCount; i++)
                {
                    // Instantiate bullet at the current position
                    
                   
                    bullets[i] = Instantiate(bullet, origin, Quaternion.identity);

                    // Calculate spread angle for each bullet
                    float angleOffset = ((i / (float)(bulletCount - 1)) - 0.5f) * coneAngle;

                    // Rotate the base direction by the computed angle
                    Vector3 spreadDirection = Quaternion.Euler(0, angleOffset, 0) * baseDirection;

                    // Apply velocity or movement logic
                    Rigidbody rb = bullets[i].GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = spreadDirection * bulletSpeed;
                    }
                }
                setGunPoint();

                return 2.5f;
        }

        public void OnTimerEnd(int data)
        {

            ShootBullets();

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

        private void FixedUpdate()
        {
            waitDuration -= Time.deltaTime; // Subtract elapsed time
            if (waitDuration <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .050f);
            }
        }
        
        private void Update() {
            Vector3 targetDir = player.transform.position - transform.position;
            float step = 3 * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

}
