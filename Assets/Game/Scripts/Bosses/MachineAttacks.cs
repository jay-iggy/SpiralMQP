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
        [SerializeField] GameObject dronePrefab;
        [SerializeField] GameObject laser;
        private GameObject player;
        private List<GameObject> drones = new List<GameObject>();
        int bulletAngle = 0;
        int smokeAngle = -1; //0 is up, -1 is no smoke
        float laserTurnDelta = 0;

        void Start()
        {
            timer.onTimerEnd.AddListener(OnTimerEnd);
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
        }

        void FixedUpdate()
        {
            BulletPatterns.MoveTowards(drones.ToArray(), player.transform.position, 3);
            Quaternion laserQuat = new Quaternion();
            laserQuat.eulerAngles = new Vector3(0, laserTurnDelta, 0);
            laser.transform.rotation *= laserQuat;
        }

        public int GetAttackCount() { return 3; }

        public float Attack(int index)
        {
            curAttack = index;

            laser.SetActive(false);
            laserTurnDelta = 0;

            switch (index)
            {
                case 0:
                    return ShootBulletCircles();
                case 1:
                    return ShootLaser();              
                case 2:
                    return LaunchDrone();
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
            return 2f;
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
            GameObject newDrone = Instantiate(dronePrefab, transform.position, Quaternion.identity);
            newDrone.transform.localScale = new Vector3(1, 1, 1);
            drones.Add(newDrone);
            timer.Set(.25f, 1);
            return 2f;
        }

        private float ShootLaser()
        {
            float adjacent = player.transform.position.x;
            float opposite = player.transform.position.z;
            if (adjacent == 0) adjacent = .01f; //divide by 0 protection

            float angle = Mathf.Atan(Mathf.Abs(opposite / adjacent));
            if (opposite > 0 && adjacent > 0)
            {
                angle = -angle;
            }
            else if (opposite > 0 && adjacent < 0)
            {
                angle += Mathf.PI;
            }
            else if (opposite < 0 && adjacent < 0)
            {
                angle = -angle;
                angle += Mathf.PI;
            }
            angle *= Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            Debug.Log(angle);

            laser.transform.localEulerAngles = new Vector3(0, angle-90, 0); //starts opposite player
            if(Random.Range(0, 2) == 0)
            {
                laserTurnDelta = 2;
            }
            else
            {
                laserTurnDelta = -2;
            }

            laser.SetActive(true);

            return 2f;
        }

        private float MakeSmoke()
        {
            return 2;
        }
    }
}
