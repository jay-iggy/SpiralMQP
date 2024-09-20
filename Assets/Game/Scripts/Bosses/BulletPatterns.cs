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
                bullets[i].transform.position = center - radius * Vector3.left;
                bullets[i].transform.RotateAround(center, Vector3.up, angle);
                i++;
            }
        }

        public static void MoveTowards(GameObject[] bullets, Vector3 target, float speed) {
            foreach (GameObject bullet in bullets) {
                if(bullet == null) {
                    continue;
                }
                Vector3 v = target - bullet.transform.position;
                v.Normalize();
                v *= speed;
                bullet.GetComponent<Rigidbody>().velocity = v;
            }
        }
    }
}
