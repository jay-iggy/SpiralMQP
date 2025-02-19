using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class RatAttack : MonoBehaviour, IHaveOneAttack
    {
        private GameObject bullet;
        public void SetBullet(GameObject b) { bullet = b; }

        private Timer timer;

        private bool attacking = false;

        void Start()
        {
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
        }

        public float Attack()
        {
            attacking = true;
            GameObject bulletInChamber = Instantiate(bullet, transform.position, Quaternion.identity);
            Projectile p = bulletInChamber.GetComponent<Projectile>();
            p.TargetPlayer(8);
            p.destroyedByWall = true;
            timer.Set(.5f, 0);

            return 2;
        }

        public void StopAttacking()
        {
            attacking = false;
        }

        public void OnTimerEnd(int data)
        {
            if (attacking) { Attack(); }
        }
    }
}
