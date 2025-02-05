using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Game.Scripts
{
    public class Coward : MonoBehaviour, ICanAttack
    {
        private const int RUNAWAY = 0;
        private const int TELLAPORT = 1;
        private const int MAKE_GOON = 2;

        [SerializeField] GameObject bullet;
        [SerializeField] Timer timer;
        private GameObject player;

        [SerializeField] GameObject Goon1;
        [SerializeField] GameObject Goon2;
        [SerializeField] GameObject Goon3;

        private int shotsInChamber;
        private GameObject bulletInChamber; // we can have this be type Projectile

        private Vector3 center = new Vector3(0, 2, 0);
        private float speed;

        private int curAttack = -1;

        private Vector3 gun;
        private Vector3 bulletCircleCenter;

        private int goonTracker;



        //audio
        public AudioManager AudioCON;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);
            gun = transform.position + Vector3.left;
            goonTracker = 1;
            MakeGoon();
        }



        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {
            
            curAttack = index;
            switch (curAttack)
            {
                case RUNAWAY:
                    Debug.Log("case1");
                    return run();
                case TELLAPORT:
                    Debug.Log("case2");
                    return Tellaport();
                case MAKE_GOON:
                    Debug.Log("case3");
                    return MakeGoon();
            }

            return 0;
        }

       
        private void CreatStartGoons()
        {

            Vector3 spawnPosition = transform.position + Vector3.right * 2f; // 2 units to the right
            GameObject GoonA = Instantiate(Goon1, spawnPosition, Quaternion.identity);
            /*
            spawnPosition = transform.position + Vector3.left * 2f; // 2 units to the right
            GameObject GoonB = Instantiate(Goon2, transform.position, Quaternion.identity);
            */
        }



        private float run()
        {
            timer.Set(1, 0);
            return 4f;
        }

        private float Tellaport()
        {

            transform.position = BossRoom.GetRandomPositionInRoom();
            timer.Set(2, 1);
            return 1f;
        }

        private float MakeGoon()
        {
            float maxhealth = GetComponent<HealthComponent>().maxHealth;
            float curhealth = GetComponent<HealthComponent>().health;
            int x= GetHealthStage(maxhealth, curhealth)- CountGoons();
            Debug.Log(maxhealth);
            Debug.Log(curhealth);
            Debug.Log(GetHealthStage(maxhealth, curhealth));
            Debug.Log(CountGoons());
            Debug.Log(x);
                for (int i = 0; i < x; i++)
            {
                GameObject newGoon=null;
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

        public int CountGoons()
        {

            // Find all objects with the Goon component
            Goon[] goons = FindObjectsOfType<Goon>();

            // Return the count
            return goons.Length;
        }

        private int GetHealthStage( float maxHealth, float currentHealth)
        {
            // Avoid division by zero
            if (maxHealth <= 0) return 0;

            // Calculate health fractions
            float healthFraction = currentHealth / maxHealth;

            // Determine health stage based on thresholds
            if (healthFraction <= 1f / 6f) return 7;
            if (healthFraction <= 2f / 6f) return 6;
            if (healthFraction <= 3f / 6f) return 5;
            if (healthFraction <= 4f / 6f) return 4;
            if (healthFraction <= 5f / 6f) return 3;

            return 2; // For full health or invalid cases
        }
        public Vector3 GetRandomPointOnCircle(int r)
        {
            // Generate a random angle in radians
            float angle = Random.Range(0f, Mathf.PI * 2);

            // Calculate x and z coordinates relative to the circle's center
            float x = transform.position.x + r * Mathf.Cos(angle);
            float z = transform.position.z + r * Mathf.Sin(angle);

            // Return the new point, keeping the original y value
            return new Vector3(x, transform.position.y, z);
        }


        private float GoToCenter()
        {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.Set(1, 2);
            return 2.25f;
        }


        

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case RUNAWAY:
                    
                    curAttack = -1;
                    break;
                case TELLAPORT:
                    break;
                case MAKE_GOON:
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

        private void FixedUpdate()
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
            transform.position += directionAwayFromPlayer * 0.025f;
            
        }
        public void DestroyAllGoons()
        {
            // Find all active GameObjects with the Goon component
            Goon[] goons = FindObjectsOfType<Goon>();

            // Loop through and destroy each one
            foreach (Goon goon in goons)
            {
                Destroy(goon.gameObject);
                Debug.Log("Destroyed GameObject: " + goon.gameObject.name);
            }
        }
    }

}
