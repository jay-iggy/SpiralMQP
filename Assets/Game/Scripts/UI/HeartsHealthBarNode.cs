using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    [RequireComponent(typeof(Animator))]
    public class HeartsHealthBarNode : MonoBehaviour {
        [SerializeField] Image heartImage;
        public bool isEnabled { get; private set; } = true;
        private HealthComponent _healthComponent;
        private Animator _animator;
        
        [SerializeField] private float lowHealthPercent = .3f;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        public void SetHealthComponent(HealthComponent healthComponent) {
            _healthComponent = healthComponent;
        }

        public void UpdateLowHealthAnimation() {
            _animator.SetBool("isLowHealth", _healthComponent.health / _healthComponent.maxHealth < lowHealthPercent);
        }

        
        public void SetHeartEnabled(bool isEnabled) {
            this.isEnabled = isEnabled;
            _animator.SetBool("isEmpty", !isEnabled);
            //TODO: animate appearing or disappearing
        }
        
    }
}