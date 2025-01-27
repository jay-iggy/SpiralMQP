using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts {
    public class DeltaHealthBar : MonoBehaviour {
        [SerializeField] private HealthBar healthBar;
        private HealthComponent healthComponent;
        [SerializeField] private Slider slider;
        [Space]
        [SerializeField] private float hesitateDuration = 0.2f;
        [SerializeField] private float decreaseRate = 40f;

        private bool _followerIsActive = false;
        private float _deltaAmount = 0f;
        
        private float _hesitateTimer = 0f;

        private void Awake() {
            if (healthBar != null) {
                healthBar.onBindHealthComponent.AddListener(SetHealthComponent);
            }
        }

        private void OnEnable() {
            if(healthComponent != null) {
                healthComponent.onTakeDamageFloat.AddListener(OnTakeDamage);
            }
        }

        private void OnDisable() {
            if(healthComponent != null) {
                healthComponent.onTakeDamageFloat.RemoveListener(OnTakeDamage);
            }
        }

        private void Start() {
            slider.gameObject.SetActive(false);
        }
        
        private void SetHealthComponent(HealthComponent newHealthComponent) {
            healthComponent = newHealthComponent;
            healthComponent.onTakeDamageFloat.AddListener(OnTakeDamage);
        }
        
        private void OnTakeDamage(float damage) {
            // if bar is not active, start delta bar
            if (!_followerIsActive) {
                StartUpDeltaBar(damage);
                return;
            }
            
            // if delta bar is active but still hesitating, add new damage to existing delta bar
            if(_followerIsActive && Time.time < _hesitateTimer) {
                _deltaAmount += damage;
                UpdateSlider();
                _hesitateTimer = Time.time + hesitateDuration; // restart timer
                return;
            }
            
            // if delta bar is not hesitating, reset bar with new value
            if(_followerIsActive && Time.time >= _hesitateTimer) {
                _deltaAmount = damage;
                _hesitateTimer = Time.time + hesitateDuration; // restart timer
                UpdateSlider();
            }
        }

        private void StartUpDeltaBar(float damage) {
            _followerIsActive = true;
            _deltaAmount = damage;
            
            _hesitateTimer = Time.time + hesitateDuration;
            
            slider.gameObject.SetActive(true);
            UpdateSlider();
        }
        
        private void Update() {
            // if hesitate timer is over, tick down delta bar
            if(Time.time >= _hesitateTimer) {
                _deltaAmount -= decreaseRate * Time.deltaTime;
                UpdateSlider();
            }
        }

        private void UpdateSlider() {
            if(healthComponent == null) {
                // prevents null reference exception
                // but freezes the delta bar when health component is deleted during transition
                return;
            }
            
            // update ui element
            slider.value = (_deltaAmount + healthComponent.health)/healthComponent.maxHealth;
            
            // if bar is fully drained, shut down
            if (_deltaAmount <= 0) {
                _followerIsActive = false;
                slider.gameObject.SetActive(false);
                _deltaAmount = 0;
            }
        }

        
    }
}