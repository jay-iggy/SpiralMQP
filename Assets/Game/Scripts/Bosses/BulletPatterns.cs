using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public static class BulletPatterns : object
    {
        public static void circle(GameObject[] bullets, Vector3 center, float radius, int startingDegrees=0)
        {
            float angle = 360 / bullets.Length;
            int i = 0;
            for(float a = startingDegrees; a<360+startingDegrees; a += angle)
            {
                bullets[i].transform.position = center - radius * Vector3.left;
                bullets[i].transform.RotateAround(center, Vector3.up, a);
                i++;
            }
        }

        public static void moveTowards(GameObject[] bullets, Vector3 target, float speed)
        {
            for(int i = 0; i<bullets.Length; i++)
            {
                Vector3 v = target - bullets[i].transform.position;
                v.Normalize();
                v *= speed;
                bullets[i].GetComponent<Rigidbody>().velocity = v;
            }     
        }
    }

}
