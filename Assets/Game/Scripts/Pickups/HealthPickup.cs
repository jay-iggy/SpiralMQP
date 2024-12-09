using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Pickups
{
    public class HealthPickup : ItemPickup {
        [SerializeReference] private Sound sfx;

        private enum HealthType
        {
            add,
            addToMax,
            refill
        }
        [SerializeField] private HealthType type;
        [SerializeField] private int amount;

        public HealthPickup()
        {
            itemType = ItemType.HEALTH;
        }
        protected override void ApplyEffect(PlayerController player)
        {
            HealthComponent healthComponent = player.GetHealthComponent();

            switch (type) {
                case HealthType.add:
                    healthComponent.Heal(amount);
                    if (sfx != null) sfx.PlaySound();
                    break;
                case HealthType.addToMax:
                    healthComponent.SetMaxHealth(healthComponent.maxHealth + amount);
                    healthComponent.Heal(amount);
                    if (sfx != null) sfx.PlaySound();
                    break;
                case HealthType.refill:
                    healthComponent.SetHealth(healthComponent.maxHealth);
                    if (sfx != null) sfx.PlaySound();
                    break;
            }
        }
    }
}
