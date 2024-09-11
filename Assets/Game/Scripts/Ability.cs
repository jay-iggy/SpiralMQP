using UnityEngine;

namespace Game.Scripts {
    public abstract class Ability : MonoBehaviour {
        protected PlayerController _player;
        public void BindToPlayer(PlayerController player) {
            _player = player;
        }
        
        public abstract void AbilityPressed();
        public abstract void AbilityReleased();
    }
}