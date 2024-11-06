using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Pickups
{
    public class PassivePickup : ItemPickup
    {
        public GameObject ability;
        protected override void ApplyEffect(PlayerController player)
        {
            Instantiate(ability, player.transform.GetChild(1));
        }
    }
}
