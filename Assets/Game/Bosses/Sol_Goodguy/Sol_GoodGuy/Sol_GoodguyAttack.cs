using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Sol_GoodguyAttack : MonoBehaviour, ICanAttack
    {
        private const int GUN_FLAME = 0;
        private const int RIOT_STOMP = 1;
        private const int FIRE_BALL = 2;
        private const int CHARGE_DRAGON_INSTALL = 3;
        private const int DRAGON_INSTALL= 4;

        [SerializeField] GameObject ExplosiveFlame;
        [SerializeField] GameObject Flame;
        [SerializeField] GameObject Trial;
        [SerializeField] Timer timer;
        private GameObject player;

        private bool MakeTrail;

        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private Vector3 center = new Vector3(0, 2, 0);
        GameObject[] bullets = new GameObject[12]; // why not just use a list?

        public int curAttack = -1;

        public Vector3 StompLocation;

        public bool DragonInstall;

        public float speed = 5f;                // Speed of the boss movement
        public float changeDirectionInterval = 3f; // Time interval for changing direction
        public Vector3 movementBounds = new Vector3(10f, 0f, 10f); // Boundary for movement

        private Vector3 moveDirection;
        private float timeToChangeDirection;

        public bool IsOnWall;
        public int ThePhase;
        private int location;
        private bool Istrail;
        private float movetimer = 0f;
        private float movetimer2 = 0f;
        private float trailtime = 0f;
        private bool reload;


        //audio
        public AudioManager AudioCON;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            DragonInstall = false;
            IsOnWall =false;
            Istrail =false;
            ThePhase = 0;
            curAttack = 0;
            trailtime = 0.5f;
            reload =true;

            // Find the GameObject by name in the scene

        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {
            

            switch (curAttack)
            {
                case -1:
                    break;
                case GUN_FLAME:
                    return GunFlame();
                case RIOT_STOMP:
                    return RiotStomp();
                case FIRE_BALL:
                    if (reload)
                    {
                        shotsInChamber = 4;
                    }
                    return FireBall();
                case CHARGE_DRAGON_INSTALL:
                    return GoToCenter() ;
                case DRAGON_INSTALL:
                    return TransformAttack();
            }

            Debug.Log(curAttack);
            return 0;
        }



        private float GunFlame()
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
                if (DragonInstall && (i % 2 == 0)) {

                    bullets[i] = Instantiate(ExplosiveFlame, origin, Quaternion.identity);
                }
                else
                {
                    bullets[i] = Instantiate(Flame, origin, Quaternion.identity);
                }
                

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
            curAttack = 1;
            if (DragonInstall)
            {
                return 2;
            }
            return 1;
        }



        private float RiotStomp()
        {
            
                switch (ThePhase)
                {
                case 0:
                    location = Random.Range(1, 5);

                    StompLocation = SetRandomDirection(location);
                    ThePhase = 1;
                    timer.Set(.5f, 1);
                    break;
                case 1:
                        if (IsOnWall)
                        {
                        switch (location)
                        {
                            case 1:
                                location = 4;
                                break;
                            case 2:
                                location = 3;
                                break;
                            case 3:
                                location = 2;
                                break;
                            case 4:
                                location = 1;
                                break;
                        }
                        StompLocation = SetRandomDirection(location);
                        ThePhase = 2;
                        Istrail = true;
                    }
                    timer.Set(.5f, 1);
                    break;
                    case 2:

                    
                    IsOnWall = false;
                   
                    ThePhase = 3;
                    timer.Set(.5f, 1);
                    break;
                    case 3:
                    
                    if (IsOnWall)
                    {
                        ThePhase = 0;
                        Istrail = false;
                        curAttack = 2;
                    }
                    timer.Set(.5f, 1);
                    return 1f;


                }
            

            return 1f;
        }

        private float GoToCenter()
        {

            if(transform.position == center)
            {
                curAttack = 4;
                return 1.5f;
            }
            
            return 1f;
        }

        private float FireBall()
        {
            if (shotsInChamber > 0)
            {
                reload = false;
                if (DragonInstall)
                {
                    bulletInChamber = Instantiate(ExplosiveFlame, transform.position, Quaternion.identity);
                }
                else { 
                    bulletInChamber = Instantiate(Flame, transform.position, Quaternion.identity); 
                }
                
                bulletInChamber.GetComponent<Projectile>().TargetPlayer(25);
            }

            if (shotsInChamber >= 0)
            {
                shotsInChamber--;
                timer.Set(.01f, 2);
            }
            else
            {
                reload= true;
                curAttack = 0;
                bulletInChamber = null;
            }
            if (DragonInstall)
            {
                return 3f;
            }
            return 1f;
        }
        private int TransformAttack()
        {
            
                for (int i = 0; i < 12; i++)
                {
                    bullets[i] = Instantiate(Flame);
                }
                BulletPatterns.CreateCircle(bullets, transform.position, 1);
                BulletPatterns.MoveTowards(bullets, transform.position, -8);
                DragonInstall = true;
            curAttack = 1;
            return 2;
            
        }
        private void bombs()
        {
            GameObject bullets = Instantiate(Trial, BossRoom.GetRandomPositionInRoom(), Quaternion.identity);
        }
        

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case GUN_FLAME:

                    break;

                case RIOT_STOMP:
                    
                    break;
                case FIRE_BALL:
                    
                    break;

            }
        }
        private void TransformDragonInstall()
        {
            float maxhealth = GetComponent<HealthComponent>().maxHealth;
            if((GetComponent<HealthComponent>().maxHealth/2) > GetComponent<HealthComponent>().health)
            {
                DragonInstall = true;
                trailtime = 0.75f;
                curAttack = 3;
                if (transform.position == center)
                {
                    TransformAttack();
                }
                
            }

           
        }
        private Vector3 SetRandomDirection(int location)
        {
            return BossRoom.GetRandomPositionOnWallcorner(location);
            
        }

        private void makeTrail()
        {
            if (DragonInstall)
            {
                Vector3 leftOffset = transform.right * -1f; // Left of the object
                Vector3 rightOffset = transform.right;     // Right of the object

                // Spawn left bullet
                GameObject leftBullet = Instantiate(Flame, transform.position + leftOffset, Quaternion.identity);
                leftBullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, leftOffset);
                BulletPatterns.MoveTowards(leftBullet, transform.position, -3);
                // Spawn right bullet
                GameObject rightBullet = Instantiate(leftBullet, transform.position + rightOffset, Quaternion.identity);
                rightBullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, rightOffset);
                BulletPatterns.MoveTowards(rightBullet, transform.position, -3);
            }
            else
            {
                GameObject bullets = Instantiate(Trial, transform.position, Quaternion.identity);
            }
            
        }
        void OnCollisionEnter(Collision collision)
        {
            // Check if the other object has the "Wall" tag
            if (collision.gameObject.CompareTag("Wall"))
            {
                IsOnWall = true;
                
            }
        }
        void OnCollisionExit(Collision collision)
        {
            // Check if the object stopped colliding with an object tagged "Wall"
            if (collision.gameObject.CompareTag("Wall"))
            {
                IsOnWall = false;
            }
        }

        private void FixedUpdate()
        {
            
            if (DragonInstall)
            {

                movetimer2 += Time.deltaTime;
                // Check if one second has passed
                if (movetimer2 >= 0.5f)
                {

                    bombs();
                    // Reset the timer
                    movetimer2 = 0f;
                }
            }
            else
            {
                TransformDragonInstall();
            }

            if (Istrail)
            {
                movetimer += Time.deltaTime;


                // Check if one second has passed

                if (movetimer >= trailtime)
                {

                    makeTrail();
                    // Reset the timer
                    movetimer = 0f;
                }
            }
            

                
            

            switch (curAttack)
            {
                case GUN_FLAME:
                    // move to center before shooting
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.005f);
                    // Calculate the direction to the target
                    Vector3 direction3 = StompLocation - transform.position;

                    if (direction3 != Vector3.zero)
                    {
                        // Create a rotation that looks in the movement direction
                        Quaternion targetRotation = Quaternion.LookRotation(direction3);

                        // Apply the rotation, adjusting all axes (X, Y, Z)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
                    }
                    break;
                case RIOT_STOMP:
                    transform.position = Vector3.MoveTowards(transform.position, StompLocation, 0.1f);
                    // Calculate the direction to the target
                    Vector3 direction = StompLocation - transform.position;

                    if (direction != Vector3.zero)
                    {
                        // Create a rotation that looks in the movement direction
                        Quaternion targetRotation = Quaternion.LookRotation(direction);

                        // Apply the rotation, adjusting all axes (X, Y, Z)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
                    }
                    break;
                case FIRE_BALL:
                    // move to center before shooting
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.05f);
                    // Calculate the direction to the target

                    // Calculate the direction vector to the target
                    Vector3 direction1 = StompLocation - transform.position;

                    if (direction1 != Vector3.zero)
                    {
                        // Create a rotation that looks in the movement direction
                        Quaternion targetRotation = Quaternion.LookRotation(direction1);

                        // Apply the rotation, adjusting all axes (X, Y, Z)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
                    }
                    break;
                case CHARGE_DRAGON_INSTALL:
                    // move to center before shooting
                    transform.position = Vector3.MoveTowards(transform.position, center, 0.1f);
                    // Calculate the direction to the target
                    Vector3 direction2 =center - transform.position;

                    if (direction2 != Vector3.zero)
                    {
                        // Create a rotation that looks in the movement direction
                        Quaternion targetRotation = Quaternion.LookRotation(direction2);

                        // Apply the rotation, adjusting all axes (X, Y, Z)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
                    }

                    break;
            }

        }
    }

}
