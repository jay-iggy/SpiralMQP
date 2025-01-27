using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts {
    public class HealthBar : MonoBehaviour {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Slider healthBar;
        [SerializeField] private float refillLerpSpeed = 2;
        public UnityEvent<HealthComponent> onBindHealthComponent = new();

        private void Start() {
            if (healthComponent != null) {
                BindHealthComponent(healthComponent);
            }
        }

        // TODO: with this game concept, we'll need to swap this health component pretty often
        public void BindHealthComponent(HealthComponent healthComponent) {
            this.healthComponent = healthComponent;
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            onBindHealthComponent.Invoke(healthComponent);
            StartCoroutine(LerpHealthBar(healthComponent.health, refillLerpSpeed));
        }

        private void UpdateHealthBar(float health) {
            healthBar.value = health / healthComponent.maxHealth;
        }
        
        private IEnumerator LerpHealthBar(float targetValue, float lerpSpeed) {
            float startValue = healthBar.value;
            float t = 0;
            while (t<1) {
                t += Time.deltaTime * lerpSpeed;
                UpdateHealthBar(Mathf.Lerp(startValue, targetValue, t));
                yield return null;
            }
        }
    }
}