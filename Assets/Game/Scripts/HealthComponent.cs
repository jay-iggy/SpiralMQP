using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Game.Scripts {
    public class HealthComponent : MonoBehaviour, ICanGetHit {
        public UnityEvent<float> onHealthChanged;
        public UnityEvent onDeath;
        public UnityEvent onTakeDamage;

        //Damage Display Stuff
        public GameObject FlotingTextPrefab;

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

            //Display Damage taken
            if(FlotingTextPrefab != null){
                ShowFloatingText();
            }
           

            if (health <= 0) {
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

        public void ShowFloatingText()
        {
            //Creat text
            var go = Instantiate(FlotingTextPrefab, transform.position,Quaternion.identity,transform);
            go.GetComponent<TMP_Text>().text = "1"; //set value, Currently only show 1
        }
    }
}