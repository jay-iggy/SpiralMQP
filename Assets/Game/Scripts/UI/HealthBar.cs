using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    public class HealthBar : MonoBehaviour {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Slider healthBar;

        private void Start() {
            if(healthComponent != null) {
                BindHealthComponent(healthComponent);
            }
        }
        
        // TODO: with this game concept, we'll need to swap this health component pretty often
        public void BindHealthComponent(HealthComponent healthComponent) {
            this.healthComponent = healthComponent;
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(healthComponent.health);
        }

        private void UpdateHealthBar(float health) {
            healthBar.value = health / healthComponent.maxHealth;
        }
    }
}