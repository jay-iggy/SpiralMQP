using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Pickups
{
    public class HealthPickup : ItemPickup
    {
        protected override void ApplyEffect(PlayerController player)
        {
            HealthComponent healthComponent = player.GetHealthComponent();
            healthComponent.SetHealth(100);
        }
    }
}
