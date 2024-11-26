using UnityEngine;

namespace Game.Scripts {
    public abstract class Ability : MonoBehaviour {
        protected PlayerController _player;
        public void BindToPlayer(PlayerController player) {
            _player = player;
        }
        
        public abstract void AbilityPressed();
        public abstract void AbilityReleased();
        public virtual void OnAbilityUnequipped() {
            // Whenever the ability is unequipped, cleanup things that might have been cut off
            // For example: disable colliders that were enabled and need to be disabled
        }
    }
}