using System;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class HealthComponent : MonoBehaviour, ICanGetHit {
        public UnityEvent<float> onHealthChanged;
        public UnityEvent onDeath;
        public UnityEvent onTakeDamage;
        public UnityEvent<float> onTakeDamageFloat;
        
        public float invincibilityDuration = 0f;
        private float invincibleUntil = 0f;
        [SerializeField] bool affectsHitless = false;

        //Damage Display Stuff
        public FloatingText floatingTextPrefab;

        public float health { get; private set; }
        public float maxHealth = 100;
        public bool isAlive { get; private set; } = true;
        
        private void Awake() {
            health = maxHealth;
        }

        private void Start() {
            if(floatingTextPrefab != null) {
                onTakeDamageFloat.AddListener(ShowFloatingText);
            }
            onDeath.AddListener(PlayDeathJuice);
        }

        public void SetHealth(float newHealth) {
            health = Mathf.Clamp(newHealth, 0, maxHealth);
            onHealthChanged.Invoke(health);
        }
        public void TakeDamage(float damage) {
            if(isInvincible()) {
                return;
            }
            
            SetHealth(health - damage);
            onTakeDamage.Invoke();
            onTakeDamageFloat.Invoke(damage);
            
            invincibleUntil = Time.time + invincibilityDuration;

            if (health <= 0 && isAlive) {
                isAlive = false;
                onDeath.Invoke();
            }
        }

        public void GetHit(float damage) {
            if (affectsHitless) {
                StickerManager.instance.hitless = false;
            }

            TakeDamage(damage);
        }

        private void ShowFloatingText(float damage) {
            FloatingText textObj = Instantiate(floatingTextPrefab);
            textObj.transform.position = transform.position;
            textObj.SetText($"{damage}");
        }

        private void PlayDeathJuice() {
            ScreenShake.instance.StartShake(0.5f, 0.5f);
            HitPause.instance.Pause(0.35f);
        }
        
        public bool isInvincible() {
            return invincibleUntil > Time.time;
        }
    }
}