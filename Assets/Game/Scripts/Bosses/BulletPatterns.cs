using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{

    public static class BulletPatterns {

        

        public static void CreateCircle(GameObject[] bullets, Vector3 center, float radius, int startingDegrees=0) {
            float increment = 360 / bullets.Length;
            int i = 0;
            for (float angle = startingDegrees; angle < 360 + startingDegrees; angle += increment) {
                if (i<bullets.Length && bullets[i] != null)
                {
                    bullets[i].transform.position = center - radius * Vector3.left;
                    bullets[i].transform.RotateAround(center, Vector3.up, angle);
                }
                i++;
            }
        }

        public static void CreateCone(GameObject[] bullets, Vector3 center, float radius, GameObject player, int startingDegrees = 0)
        {
            float increment = 30f / bullets.Length;
            int i = 0;

            // Get the direction from the cone center to the player
            Vector3 directionToPlayer = (player.transform.position - center).normalized;

            for (float angle = startingDegrees; angle < 45 + startingDegrees; angle += increment)
            {
                if (i < bullets.Length && bullets[i] != null)
                {
                    // Set the initial position of each bullet
                    bullets[i].transform.position = center - radius * Vector3.left;

                    // Rotate the initial position around the center to create cone shape
                    bullets[i].transform.RotateAround(center, Vector3.up, angle);

                    // Calculate the bullet's new direction towards the player
                    Vector3 bulletDirection = (player.transform.position - bullets[i].transform.position).normalized;

                    // Apply velocity or movement logic to the bullet
                    float bulletSpeed = 10f;
                    Rigidbody rb = bullets[i].GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = bulletDirection * bulletSpeed;
                    }
                }
                i++;
            }
        }


        public static void MoveTowards(GameObject bullet, Vector3 target, float speed) {
            if(bullet == null) {
                Debug.LogWarning($"BulletPatterns::MoveTowards(): Bullet is null!");
                return;
            }
            Vector3 v = target - bullet.transform.position;
            v.y = 0;
            v.Normalize();
            v *= speed;
            bullet.GetComponent<Rigidbody>().velocity = v;
        }

        public static void MoveTowards(GameObject[] bullets, Vector3 target, float speed) {
            foreach(GameObject bullet in bullets) {
                MoveTowards(bullet, target, speed);
            }
        }
    }
}
