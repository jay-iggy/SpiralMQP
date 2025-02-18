using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game.Scripts
{
    public class SolPillarAttacks : MonoBehaviour, ICanAttack
    {
        [SerializeField] GameObject bullet;
        [SerializeField] GameObject[] locations;
        [SerializeField] float moveSpeed = .1f;
        [SerializeField] float moveTime = 1;
        private float trailTimer = 0;
        int currentLocation = 0;
        GameObject targetLocation;
        bool moving = false;

        void FixedUpdate()
        {
            if (!moving) return;

            trailTimer += Time.deltaTime;
            if(trailTimer > 0.2f)
            {
                trailTimer = 0;
                MakeTrail();
            }

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
                    return Move();
                case 1:
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

        private void MakeTrail()
        {
            GameObject bullets = Instantiate(bullet, transform.position, Quaternion.identity);
        }

    }
}
