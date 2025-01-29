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
        [SerializeField][GradientUsage(true)] private Gradient damagePulseGradient;
        [SerializeField] private float damagePulseDuration = .5f;
        private Coroutine _pulseDamageColorCoroutine;

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
            healthComponent.onTakeDamage.AddListener(OnTakeDamage);
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

        private void OnTakeDamage() {
            if(_pulseDamageColorCoroutine != null) {
                StopCoroutine(_pulseDamageColorCoroutine);
            }
            _pulseDamageColorCoroutine = StartCoroutine(PulseDamageColor());
        }
        private IEnumerator PulseDamageColor() {
            Image fillImage = healthBar.fillRect.GetComponent<Image>();
            
            float t = 0;
            while (t<damagePulseDuration) {
                t += Time.deltaTime;
                fillImage.color = damagePulseGradient.Evaluate(t/damagePulseDuration);
                yield return null;
            }
        }
    }
}