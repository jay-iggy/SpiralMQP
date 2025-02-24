using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Gaurd_Pistal : MonoBehaviour, ICanAttack
    {
       
        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;
        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile
        public float waitDuration = 2f;

        private Vector3 gun;
        [SerializeField] ParticleSystem muzzleFlash;
        [SerializeField] Transform projectileSpawnPoint;



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
            
                setGunPoint();
                bulletInChamber = Instantiate(bullet, projectileSpawnPoint.position, Quaternion.identity);
                muzzleFlash.Play();
                bulletInChamber.GetComponent<Projectile>().TargetPlayer(8);
                return 1;
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
