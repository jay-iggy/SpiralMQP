using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(HealthComponent))]
    public class Goon : Boss {
        


        override public void Die()
        {
            Debug.Log("Dead goon");
            isAlive = false;
            Destroy(gameObject);
        }
    }
}
