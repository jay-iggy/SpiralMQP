using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    public class StoneskinAbility : MonoBehaviour
    {
        private GameObject player;
        private HealthComponent hp;
        private PlayerController pc;
        void Start()
        {
            player = transform.parent.parent.gameObject;
            hp = player.GetComponent<HealthComponent>();
            hp.onTakeDamage.AddListener(SetStoneskin);
            pc = player.GetComponent<PlayerController>();
            pc.movementSpeed = 4.5f;
        }

        public void SetStoneskin()
        {
            if(Random.Range(0, 3) == 0)
            {
                hp.takeZeroDamage = true;
            }
            else
            {
                hp.takeZeroDamage = false;
            }
        }
    }
}
