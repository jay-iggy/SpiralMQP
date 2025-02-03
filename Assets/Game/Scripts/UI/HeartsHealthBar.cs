using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    public class HeartsHealthBar : MonoBehaviour {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] HorizontalLayoutGroup heartsLayoutGroup;
        [SerializeField] HeartsHealthBarNode heartPrefab;
        private List<HeartsHealthBarNode> hearts = new();

        private void Start() {
            CreateHearts();
            if(healthComponent != null) {
                BindHealthComponent(healthComponent);
                healthComponent.onMaxHealthChanged.AddListener(RefreshHearts);
            }
        }
        
        private void RefreshHearts(float newMaxHealth) {
            CreateHearts();
        }
        
        private void CreateHearts() {
            foreach (Transform child in heartsLayoutGroup.transform) {
                Destroy(child.gameObject);
            }
            hearts.Clear();
            for (int i = 0; i < healthComponent.maxHealth; i++) {
                HeartsHealthBarNode heart = Instantiate(heartPrefab, heartsLayoutGroup.transform);
                heart.SetHealthComponent(healthComponent);
                hearts.Add(heart);
            }
        }
        
        private void BindHealthComponent(HealthComponent healthComponent) {
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(healthComponent.health);
        }

        private void UpdateHealthBar(float health) {
            // go backwards through the list of hearts
            for (int i = hearts.Count - 1; i >= 0; i--) {
                SetHeartEnabled(i, i + 1 <= health);
            }
        }

        private void SetHeartEnabled(int index, bool isEnabled) {
            HeartsHealthBarNode heart = hearts[index];
            heart.SetHeartEnabled(isEnabled);
            heart.UpdateLowHealthAnimation();
        } 
    }
}