using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class RatAttacks : MonoBehaviour, ICanAttack
    {
        public int numAttacks() { return 3; }

        public float Attack(int index)
        {
            switch (index)
            {
                case 0:
                    return bigShot();
                case 1:
                    return sixShot();
                case 2:
                    return circleShot();
            }

            return 0;
        }

        private float bigShot()
        {
            transform.position = new Vector3(-5, 2, -10);
            return 1;
        }

        private float sixShot()
        {
            transform.position = new Vector3(-2, 2, -10);
            return 2;
        }

        private float circleShot()
        {
            transform.position = new Vector3(3, 2, -10);
            return 2;
        }
        
    }

}
