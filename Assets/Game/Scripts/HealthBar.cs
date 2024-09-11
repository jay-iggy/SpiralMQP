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
        
        private void BindHealthComponent(HealthComponent healthComponent) {
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(healthComponent.health);
        }

        private void UpdateHealthBar(float health) {
            healthBar.value = health / healthComponent.maxHealth;
        }
    }
}