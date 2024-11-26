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
        public UnityEvent<float> onMaxHealthChanged;
        
        public float invincibilityDuration = 0f;
        private float invincibleUntil = 0f;
        [SerializeField] bool affectsHitless = false;
        
        [SerializeField] Color hitTextColor = Color.white;

        //Damage Display Stuff
        public FloatingText floatingTextPrefab;

        public float health { get; private set; }
        public float maxHealth = 100; // dont use this directly, use SetMaxHealth
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

        public void SetMaxHealth(float newMaxHealth) {
            maxHealth = newMaxHealth;
            onMaxHealthChanged.Invoke(newMaxHealth);
        }
        public void SetHealth(float newHealth) {
            health = Mathf.Clamp(newHealth, 0, maxHealth);
            onHealthChanged.Invoke(health);
        }
        public void Heal(float amount) {
            SetHealth(health + amount);
        }
        public void TakeDamage(float damage, bool overrideInvincibility = false) {
            if(IsInvincible() && !overrideInvincibility) {
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

        public void GetHit(float damage, bool overrideInvincibility = false) {
            if (affectsHitless) {
                StickerManager.instance.hitless = false;
            }

            TakeDamage(damage, overrideInvincibility);
        }

        public bool CanBeHit(bool overrideInvincibility = false) {
            return isAlive && (overrideInvincibility || !IsInvincible());
        }

        private void ShowFloatingText(float damage) {
            FloatingText textObj = Instantiate(floatingTextPrefab);
            textObj.transform.position = transform.position;
            textObj.SetText($"{damage}");
            textObj.SetColor(hitTextColor);
        }

        private void PlayDeathJuice() {
            ScreenShake.instance.StartShake(0.5f, 0.5f);
            HitPause.instance.Pause(0.35f);
        }
        
        public bool IsInvincible() {
            return invincibleUntil > Time.time;
        }
    }
}