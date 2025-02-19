using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class PillarAttacks : MonoBehaviour, ICanAttack
    {
        private IHaveOneAttack pillarAttack;
        [SerializeField] GameObject bullet;
        [SerializeField] GameObject[] locations;
        [SerializeField] float moveSpeed = .1f;
        [SerializeField] float moveTime = 1;
        int currentLocation = 0;
        GameObject targetLocation;
        bool moving = false;

        void Start()
        {
            pillarAttack = GetComponent<IHaveOneAttack>();
            pillarAttack.SetBullet(bullet);
        }

        void FixedUpdate()
        {
            if (!moving) return;

            Vector3 direction = targetLocation.transform.position - transform.position;
            if (direction.magnitude < moveSpeed)
            {
                moving = false;
                return;
            }

            direction.Normalize();
            direction *= moveSpeed;
            transform.position += direction;
        }

        public int GetAttackCount() { return 2; }

        public float Attack(int index)
        {
            switch (index)
            {
                case 0:
                    return pillarAttack.Attack();
                case 1:
                    pillarAttack.StopAttacking();
                    return Move();
            }
            return 0;
        }

        private float Move()
        {
            int newLocation = currentLocation;
            while (newLocation == currentLocation)
            {
                newLocation = Random.Range(0, locations.Length);
            }
            currentLocation = newLocation;
            
            targetLocation = locations[newLocation];
            moving = true;

            return moveTime;
        }
        
    }
}
