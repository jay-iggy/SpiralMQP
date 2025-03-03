using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Wizard3 : MonoBehaviour, ICanAttack
    {
        private const int LINE = 0;
        private const int GAPS = 1;
        private const int TargetShot = 2;

        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;
        /*
        [SerializeField] GameObject Goon1;
        [SerializeField] GameObject Goon2;
        [SerializeField] GameObject Goon3;
        */
        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private int curAttack = -1;

        private Vector3 gun;
        private Vector3 bulletCircleCenter;

        public GameObject targetGameObject;
        private GameObject projectailWall1;
        private GameObject projectailWall2;
        private GameObject projectailWall3;

        public float End;
        public float targetZ = 0;
        public float speedX = 2f;
        public float speedZ = 3f;
        public float TopZ =105f;
        public float CenterZ = 100f;
        public float BottomZ=95f;

        private int LinePhase = 0;

        private bool moveUp;

        private bool vaulerible;

        private int Phaces =0;

        public UnityEvent Die = new();

        //audio
        //public AudioManager AudioCON;

        private void Start() {
            Vector3 pos = GameObject.Find("Origin").transform.position;
            pos.y = 1f;
            pos.x = 5f;
            transform.position = pos;
            targetGameObject = GameObject.Find("BackMovingWall");
            projectailWall1 = GameObject.Find("ProjectailWall_1");
            projectailWall2 = GameObject.Find("ProjectailWall_2");
            projectailWall3 = GameObject.Find("ProjectailWall_3");


            FinalPhaseController.instance.StartFinal();
            
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);

            curAttack = 0;

        TopZ = 104f;
        CenterZ = 99.5f;
        BottomZ = 95f;

        targetZ = CenterZ;

        }



        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {

            //curAttack = index;
            
            switch (curAttack)
            {
                case LINE:
                    Debug.Log("case1");
                    return Line();
                case GAPS:
                    Debug.Log("case2");
                    return Gap();
                case TargetShot:
                    Debug.Log("case3");
                    return ShootBullets();
            }

            return 0;
        }

        /*
        private float MakeGoon()
        {
            float maxhealth = GetComponent<HealthComponent>().maxHealth;
            float curhealth = GetComponent<HealthComponent>().health;
            
            for (int i = 0; i < x; i++)
            {
                GameObject newGoon = null;
                switch (goonTracker)
                {
                    case 1:
                        newGoon = Instantiate(Goon1, GetRandomPointOnCircle(2), Quaternion.identity);
                        goonTracker = 2;
                        break;
                    case 2:

                        newGoon = Instantiate(Goon2, GetRandomPointOnCircle(2), Quaternion.identity);
                        goonTracker = 3;
                        break;
                    case 3:
                        newGoon = Instantiate(Goon3, GetRandomPointOnCircle(2), Quaternion.identity);
                        goonTracker = 1;
                        break;
                }

            }

            timer.Set(0, 1);
            return 1f;
        }

        */

        
        private float Line()
        {
            switch (LinePhase)
            {
                case 0:
                    if (HasReachedTarget())
                    {
                        moveUp = Random.value > 0.5f;  // Randomly set to true or false (50% chance)
                        Debug.Log("Move Up: " + moveUp);
                        LinePhase++;
                        if (moveUp)
                        {
                            targetZ = TopZ;
                        }
                        else targetZ = BottomZ;
                        shotsInChamber = 3;
                    }
                    
                    break;
                case 1:
                    if (shotsInChamber > 0)
                    {
                        shotsInChamber--;
                        sendBulletStrait();
                    }
                    if (HasReachedTarget())
                    {
                        LinePhase = 2;
                    }
                    break;
                case 2:
                    moveUp = Random.value > 0.5f;  // Randomly set to true or false (50% chance)
                    Debug.Log("Move Up: " + moveUp);
                    LinePhase = 3;
                    if (targetZ == TopZ)
                    {
                        targetZ = BottomZ;
                    }
                    else targetZ = TopZ;
                    shotsInChamber = 5;
                    break;
                case 3:
                    if (shotsInChamber > 0)
                    {
                        shotsInChamber--;
                        sendBulletStrait();
                    }
                    if (HasReachedTarget())
                    {
                        LinePhase = 4;
                    }
                    break;
                case 4:
                    targetZ = CenterZ;
                    LinePhase = 0;
                    curAttack = 1;
                    shotsInChamber = 3;
                    break;

            }

            return 0.5f;
        }

        private float Gap()
        {
            int randomValue = Random.Range(0, 3);
            //Health.TakeDamage(999999999f);
            switch (randomValue) {
                case 0:
                    sendBulletStraight(5);
                    sendBulletStraight(4);
                    sendBulletStraight(3);
                    sendBulletStraight(2);
                    sendBulletStraight(1);
                    sendBulletStraight(0);
                    sendBulletStraight(-1);
                    break;
                
                case 1:
                    sendBulletStraight(5);
                    sendBulletStraight(4);
                    sendBulletStraight(3);
                    sendBulletStraight(2);
                    sendBulletStraight(-2);
                    sendBulletStraight(-3);
                    sendBulletStraight(-4);
                    sendBulletStraight(-5);
                    break;
            
                case 2:
                    sendBulletStraight(1);
                    sendBulletStraight(0);
                    sendBulletStraight(-1);
                    sendBulletStraight(-2);
                    sendBulletStraight(-3);
                    sendBulletStraight(-4);
                    sendBulletStraight(-5);
                    break;
            }
            if((shotsInChamber > 0))
            {
                shotsInChamber = 5;
                curAttack = 2;
            }
            return 4;
        }
        private float ShootBullets()
        {


            GameObject bulletInChamber = Instantiate(bullet, transform.position, Quaternion.identity);
            bulletInChamber.GetComponent<Projectile>().TargetPlayer(8);
            if ((shotsInChamber > 0))
            {

                curAttack = 0;
            }
            return 2;
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                /*
                case RUNAWAY:
                    
                    curAttack = -1;
                    break;
                case TELLAPORT:
                    break;
                case MAKE_GOON:
                    break;
                */
            }
        }


        void FixedUpdate()
        {

            MoveTowards();
        }

        private void sendBulletStrait()
        {
            GameObject bulletInChamber = Instantiate(bullet, transform.position, Quaternion.identity);
            Rigidbody bulletRb = bulletInChamber.GetComponent<Rigidbody>();
            float bulletSpeed = 5;

            if (bulletRb != null)
            {
                bulletRb.velocity = -transform.right * bulletSpeed; // Move bullet to the left
            }
        }

        private void sendBulletStraight(float zPosition)
        {
            zPosition = zPosition +CenterZ;
            // Set the spawn position with the given Z position while keeping X and Y the same
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, zPosition);

            // Instantiate the bullet at the specified position
            GameObject bulletInChamber = Instantiate(bullet, spawnPosition, Quaternion.identity);

            // Get the bullet's Rigidbody
            Rigidbody bulletRb = bulletInChamber.GetComponent<Rigidbody>();
            float bulletSpeed = 5f;

            // Apply velocity if Rigidbody exists
            if (bulletRb != null)
            {
                bulletRb.velocity = -transform.right * bulletSpeed; // Move bullet to the left
            }
        }
        void MoveTowards()
        {
            if (targetGameObject == null) return; // Exit if no target is assigned

            // Get the target's position
            Vector3 targetPosition = targetGameObject.transform.position;

            // Move towards the given Z coordinate instead of the target's Z position
            float newZ = Mathf.MoveTowards(transform.position.z, targetZ, speedZ * Time.deltaTime);

            // Move towards the target's X position with an offset of -5
            float newX = Mathf.MoveTowards(transform.position.x, targetPosition.x - 2, speedX * Time.deltaTime);

            // Apply the new position

            
            transform.position = new Vector3(newX, transform.position.y, newZ);
        }
        bool HasReachedTarget()
        {
            return Mathf.Abs(transform.position.z - targetZ) < 0.1f;
        }

        private void makeVaunible()
        {

        }

        private void makeSheild()
        {
            if(Phaces >= 3)
            {
                //Health.TakeDamage(99);
            }
        }
    }
     
}
