using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] private float waitBtwnFire;
        [SerializeField] private float bulletSpeed;

        [SerializeField] private GameObject bullet;
        [SerializeField] private Transform turretPoint;

        [SerializeField] private Timer timer;

        private GameObject player;

        private int shootData4timer = 10;

        // Start is called before the first frame update
        void Start()
        {
            timer.onTimerEnd.AddListener(OnTimerEnd);
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
            timer.Set(waitBtwnFire, shootData4timer);
        }

        void Update()
        {
            RotTowardsTarget(player.transform.position);
        }

        public void OnTimerEnd(int data)
        {
            if(data == shootData4timer)
            {
                // fire projectile
                GameObject singleBullet = Instantiate(bullet, turretPoint.position, Quaternion.identity);
                singleBullet.GetComponent<Projectile>().destroyedByWall = false;
                BulletPatterns.MoveTowards(singleBullet, player.transform.position, bulletSpeed);
                timer.Set(waitBtwnFire, shootData4timer);
            }
        }

        private void RotTowardsTarget(Vector3 target)
        {
            Vector3 flatTarget = new Vector3(target.x, this.transform.GetChild(0).position.y, target.z);
            Vector3 directionToTarget = (flatTarget - this.transform.GetChild(0).position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(-directionToTarget);
            this.transform.rotation = Quaternion.Slerp(this.transform.GetChild(0).rotation, lookRotation, Time.deltaTime * 10); // Smooth rotation
        }
    }
}
