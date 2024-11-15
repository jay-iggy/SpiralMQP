using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Interfaces;

namespace Game.Scripts
{
    public class MachineAttacks : MonoBehaviour, ICanAttack
    {
        private int curAttack = -1;
        [SerializeField] Timer timer;
        [SerializeField] GameObject bullet;
        private GameObject player;
        private List<GameObject> drones = new List<GameObject>();
        int bulletAngle = 0;

        void Start()
        {
            timer.onTimerEnd.AddListener(OnTimerEnd);
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
        }

        void Update()
        {
            BulletPatterns.MoveTowards(drones.ToArray(), player.transform.position, 3);
        }

        public int GetAttackCount() { return 2; }

        public float Attack(int index)
        {
            curAttack = index;
            switch (index)
            {
                case 0:
                    return ShootBulletCircles();
                case 1:
                    return LaunchDrone();                  
                case 2:
                    return ShootLaser();
                case 3:
                    return MakeSmoke();
            }
            return 0;
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    GameObject[] bullets = MakeBulletCircle();
                    BulletPatterns.MoveTowards(bullets, transform.position, -12);
                    break;
                case 1:
                    foreach(GameObject drone in drones)
                    {
                        drone.GetComponent<Projectile>().AddTag("Enemy");
                    }
                    break;
            }
        }

        private float ShootBulletCircles()
        {
            bulletAngle = Random.Range(0, 2) * 15; //0 or 15
            GameObject[] bullets = MakeBulletCircle();
            BulletPatterns.MoveTowards(bullets, transform.position, -2);
            timer.Set(1, 0);
            return 1.5f;
        }

        private GameObject[] MakeBulletCircle()
        {
            GameObject[] bullets = new GameObject[12];
            for (int i = 0; i < 12; i++)
            {
                bullets[i] = Instantiate(bullet);
            }
            BulletPatterns.CreateCircle(bullets, transform.position, .75f, bulletAngle);

            return bullets;
        }

        private float LaunchDrone()
        {
            GameObject newDrone = Instantiate(bullet, transform.position, Quaternion.identity);
            newDrone.transform.localScale = new Vector3(1, 1, 1);
            drones.Add(newDrone);
            timer.Set(.25f, 1);
            return 1.5f;
        }

        private float ShootLaser()
        {
            return 1;
        }

        private float MakeSmoke()
        {
            return 2;
        }
    }
}
