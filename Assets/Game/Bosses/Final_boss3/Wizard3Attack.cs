using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Wizard3 : MonoBehaviour, ICanAttack
    {
        private const int LINE = 0;
        private const int GAPS = 1;
        private const int MAKE_PILLER = 2;

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

        private float speed;

        private int curAttack = -1;

        private Vector3 gun;
        private Vector3 bulletCircleCenter;

        public Vector3 targetPosition;
        public float End;
        public float StartX;
        public float speedX = 2f;
        public float speedZ = 3f;
        public float TopX;
        public float CenterX;
        public float BottomX;

        public GameObject TargetObject;

        private int LinePhase = 0;

        private bool moveUp;


        //audio
        //public AudioManager AudioCON;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            SetTargetPosition();
            Vector3 targetObjectPosition = TargetObject.transform.position;
            CenterX = targetObjectPosition.x;
        }



        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {

            //curAttack = index;
            curAttack = 0;
            return 0;
            switch (curAttack)
            {
                case LINE:
                    Debug.Log("case1");
                    return Line();
                case GAPS:
                    Debug.Log("case2");
                    return 1;
                case MAKE_PILLER:
                    Debug.Log("case3");
                    return 0;
                    //return MakeGoon();
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
                    moveUp = Random.value > 0.5f;  // Randomly set to true or false (50% chance)
                    Debug.Log("Move Up: " + moveUp);
                    LinePhase++;
                    if (moveUp)
                    {
                        SetTargetX(TopX);
                    } else SetTargetX(BottomX);
                    shotsInChamber = 6;
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
                    if (moveUp)
                    {
                        SetTargetX(BottomX);
                    }
                    else SetTargetX(TopX);
                    shotsInChamber = 6;
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
            }

            return 4f;
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
            SetTargetZ();
            MoveObjectToTarget();
        }


        // Function to change only the X target position
        public void SetTargetX(float newX)
        {
            targetPosition = new Vector3(newX, targetPosition.y, targetPosition.z);
        }

        public void SetTargetZ()
        {
            Vector3 targetObjectPosition = TargetObject.transform.position;
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetObjectPosition.z);
        }
        public void SetTargetZ(float newZ)
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, newZ);
        }
        public void SetTargetPosition()
        {
            if (TargetObject != null)
            {
                Vector3 targetObjectPosition = TargetObject.transform.position;
                targetPosition = new Vector3(targetObjectPosition.x, targetPosition.y, targetObjectPosition.z);
            }
            else
            {
                Debug.LogWarning("Target object is null!");
            }
        }
        private bool HasReachedTarget()
        {
            // Check if the distance to the target position is within the tolerance range
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                return true;
            }
            return false;
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
        void MoveObjectToTarget()
        {
            // Get current position
            Vector3 currentPosition = transform.position;

            // Interpolate x and z positions separately with different speeds
            float newX = Mathf.MoveTowards(currentPosition.x, targetPosition.x, speedX * Time.deltaTime);
            float newZ = Mathf.MoveTowards(currentPosition.z, targetPosition.z, speedZ * Time.deltaTime);

            // Update the position (keep the y position the same)
            transform.position = new Vector3(newX, currentPosition.y, newZ);
        }
    }
     
}
