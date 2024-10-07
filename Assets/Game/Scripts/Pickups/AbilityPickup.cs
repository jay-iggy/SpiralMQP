using UnityEngine;

namespace Game.Scripts.Pickups {
    public class AbilityPickup : ItemPickup {
        //TODO: create another script that creates ability pickups given ability, slot, and gfx so we dont need to create a prefab for each ability
        
        public Ability ability;
        public AbilitySlot slot;
        
        
        protected override void ApplyEffect(PlayerController player) {
            Ability a = Instantiate(ability, player.transform);
            
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
        Secondary
    }
}