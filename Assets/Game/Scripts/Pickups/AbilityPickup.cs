using UnityEngine;

namespace Game.Scripts.Pickups {
    public class AbilityPickup : ItemPickup {
        //TODO: create another script that creates ability pickups given ability, slot, and gfx so we dont need to create a prefab for each ability
        
        public Ability ability;
        public AbilitySlot slot;

        public GameObject pickupUIprefab;

        public void Start()
        {
            base.Start();
            GameObject pickupUI = Instantiate(pickupUIprefab, this.transform);
            pickupUI.GetComponent<PickupUI>().updateValues(itemName, itemType, itemRarity);           
        }

        protected override void ApplyEffect(PlayerController player) {
            Ability a = Instantiate(ability, player.transform.GetChild(1));
            
            switch (slot) {
                case AbilitySlot.Primary:
                    player.SetPrimaryAbility(a);
                    break;
                case AbilitySlot.Secondary:
                    player.SetSecondaryAbility(a);
                    break;
            }
        }
    }
    
    public enum AbilitySlot {
        Primary,
        Secondary,
        Passive
    }
}