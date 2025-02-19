using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class MinotaurAttack : MonoBehaviour, IHaveOneAttack
    {
        private GameObject bullet;
        public void SetBullet(GameObject b) { bullet = b; }

        private float shootTimer = 0;
        [SerializeField] float fireSpeed = .08f;
        [SerializeField] MinotaurFlail flail;


        public float Attack()
        {
            flail.StartSpinning();
            return 1;
        }

        public void StopAttacking()
        {
            flail.StopSpinning();
        }

        
    }
}
