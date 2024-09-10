namespace Game.Scripts.Abilities {
    public class RunAbility : Ability {
        public override void AbilityPressed() {
            _player.isRunning = true;
            _player.movementSpeed = _player.runSpeed;
        }

        public override void AbilityReleased() {
            _player.isRunning = false;
            _player.movementSpeed = _player.walkSpeed;
        }
    }
}