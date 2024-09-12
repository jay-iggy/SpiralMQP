using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts {
    public class HealthComponent : MonoBehaviour, ICanGetHit {
        public UnityEvent<float> onHealthChanged;
        public UnityEvent onDeath;
        public UnityEvent onTakeDamage;

        public float health { get; private set; }
        public float maxHealth = 100;
        
        private void Awake() {
            health = maxHealth;
        }

        public void SetHealth(float newHealth) {
            health = Mathf.Clamp(newHealth, 0, maxHealth);
            onHealthChanged.Invoke(health);
        }
        public void TakeDamage(float damage) {
            SetHealth(health - damage);
            onTakeDamage.Invoke();
            if(health <= 0) {
                onDeath.Invoke();
                
                // TODO: move this out of here
                ScreenShake.instance.StartShake(0.5f, 0.5f);
                HitPause.instance.Pause(0.35f);
            }
        }

        public void GetHit(float damage) {
            TakeDamage(damage);
            Debug.Log(damage + " damage taken!");
        }
    }
}