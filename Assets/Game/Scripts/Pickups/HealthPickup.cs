using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Pickups
{
    public class HealthPickup : ItemPickup {
        [SerializeReference] private Sound sfx;
        public HealthPickup()
        {
            itemType = ItemType.HEALTH;
        }
        protected override void ApplyEffect(PlayerController player)
        {
            HealthComponent healthComponent = player.GetHealthComponent();
            healthComponent.SetHealth(healthComponent.maxHealth);
            if(sfx != null) sfx.PlaySound();
        }
    }
}
