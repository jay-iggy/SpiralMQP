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
        [Header("Events")]
        public UnityEvent<HealthComponent> onBindHealthComponent = new();
        [Header("Damage Color Pulse")]
        [SerializeField][GradientUsage(true)] private Gradient damagePulseGradient;
        [SerializeField] private float damagePulseDuration = .5f;
        private Coroutine _pulseDamageColorCoroutine;
        [Header("Underlay")] 
        [SerializeField] private Image underlayImage;
        [SerializeField] private AnimationCurve underlayOpacityCurve;
        private Coroutine _pulseUnderlayCoroutine;
        [SerializeField] private AnimationCurve healthUnderlayMultiplierCurve;

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
            StartCoroutine(LerpHealthBar(refillLerpSpeed));
            healthComponent.onTakeDamage.AddListener(OnTakeDamage);
        }

        private void UpdateHealthBar(float health) {
            healthBar.value = health / healthComponent.maxHealth;
        }
        
        private IEnumerator LerpHealthBar(float lerpSpeed) {
            float startValue = healthBar.value;
            float t = 0;
            while (t<1) {
                t += Time.deltaTime * lerpSpeed;
                UpdateHealthBar(Mathf.Lerp(startValue, healthComponent.health, t));
                yield return null;
            }
        }

        private void OnTakeDamage() {
            if(_pulseDamageColorCoroutine != null) {
                StopCoroutine(_pulseDamageColorCoroutine);
            }
            _pulseDamageColorCoroutine = StartCoroutine(PulseDamageColor());
            
            if(_pulseUnderlayCoroutine != null) {
                StopCoroutine(_pulseUnderlayCoroutine);
            }
            _pulseUnderlayCoroutine = StartCoroutine(PulseUnderlay());
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

        private IEnumerator PulseUnderlay() {
            float t = 0;
            while (t < damagePulseDuration) {
                t += Time.deltaTime;
                underlayImage.color = new Color(underlayImage.color.r, underlayImage.color.g, underlayImage.color.b,
                    underlayOpacityCurve.Evaluate(t / damagePulseDuration) * healthUnderlayMultiplierCurve.Evaluate(healthComponent.health/healthComponent.maxHealth));
                yield return null;
            }
        }
    }
}