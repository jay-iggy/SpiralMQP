using System;
using UnityEngine;

namespace Game.Scripts {
    public class HealthBar : MonoBehaviour {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private RectTransform healthBar;
        [SerializeField] private float maxWidth = 100;

        private void Awake() {
            healthComponent = GetComponent<HealthComponent>();
        }

        private void Start() {
            BindHealthComponent(healthComponent);
        }
        
        private void BindHealthComponent(HealthComponent healthComponent) {
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(healthComponent.health);
        }

        private void UpdateHealthBar(float health) {
            healthBar.sizeDelta = new Vector2(maxWidth * health / healthComponent.maxHealth, healthBar.sizeDelta.y);
        }
    }
}