using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Interfaces;

namespace Game.Scripts
{
    public class MachineAttacks : MonoBehaviour, ICanAttack
    {
        private const int CIRCLE_ATTACK = 0;
        private const int LASER_ATTACK = 1;
        private const int DRONE_ATTACK = 2;
        private const int SMOKE_ATTACK = 3;
        
        [SerializeField] Timer timer;
        [SerializeField] GameObject bullet;
        [SerializeField] GameObject dronePrefab;
        [SerializeField] GameObject laser;
        [SerializeField] GameObject smokePrefab;
        private GameObject player;
        private List<GameObject> drones = new List<GameObject>();
        int bulletAngle = 0;
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

        public int GetAttackCount() { return 4; }

        public float Attack(int index)
        {

            laser.SetActive(false);
            laserTurnDelta = 0;

            switch (index)
            {
                case CIRCLE_ATTACK:
                    return ShootBulletCircles();
                case LASER_ATTACK:
                    return ShootLaser();              
                case DRONE_ATTACK:
                    return LaunchDrone();
                case SMOKE_ATTACK:
                    return MakeSmoke();
            }
            return 0;
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case CIRCLE_ATTACK:
                    GameObject[] bullets = MakeBulletCircle();
                    BulletPatterns.MoveTowards(bullets, transform.position, -12);
                    break;
                case LASER_ATTACK:
                    foreach(GameObject drone in drones)
                    {
                        if(drone != null) {
                            drone.GetComponent<Projectile>().AddTag("Enemy");
                        }
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
            GameObject newDrone = Instantiate(dronePrefab, new Vector3(transform.position.x, dronePrefab.transform.position.y, transform.position.z), Quaternion.identity);
            newDrone.transform.localScale = new Vector3(1, 1, 1);
            drones.Add(newDrone);
            timer.Set(.25f, 1);
            return 1;
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
            GameObject smoke = Instantiate(smokePrefab);
            MachineSmoke ms = smoke.GetComponent<MachineSmoke>();

            switch (Random.Range(0, 5))
            {
                case 0:
                    smoke.transform.position = transform.position;
                    ms.GoTo(transform.position, new Vector3(12.4f, 1, 6.2f));
                    break;
                case 1:
                    smoke.transform.position = new Vector3(-1.88f, .4f, 0);
                    ms.GoTo(new Vector3(-6, 0, 0), new Vector3(8, 1, 11.1f));
                    break;
                case 2:
                    smoke.transform.position = new Vector3(1.88f, .4f, 0);
                    ms.GoTo(new Vector3(6, 0, 0), new Vector3(8, 1, 11.1f));
                    break;
                case 3:
                    smoke.transform.position = new Vector3(0, .4f, -1.3f);
                    ms.GoTo(new Vector3(0, 0, -3.4f), new Vector3(20.7f, 1, 3.7f));
                    break;
                case 4:
                    smoke.transform.position = new Vector3(0, .4f, 1.3f);
                    ms.GoTo(new Vector3(0, 0, 3.4f), new Vector3(20.7f, 1, 3.7f));
                    break;
            }

            return 2;
        }
    }
}
