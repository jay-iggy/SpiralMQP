using UnityEngine;

namespace Game.Scripts.Abilities {
    public class RunAbility : Ability {
        // TODO: make runSpeed a percentage rather than a flat value
        public float runSpeed = 8;
        [HideInInspector] public bool isRunning;
        public override void AbilityPressed() {
            isRunning = true;
            _player.movementSpeed = runSpeed;
        }

        public override void AbilityReleased() {
            isRunning = false;
            _player.movementSpeed = _player.walkSpeed;
        }
    }
}