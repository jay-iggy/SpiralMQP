using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    public class HeartsHealthBar : MonoBehaviour {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] HorizontalLayoutGroup heartsLayoutGroup;
        [SerializeField] GameObject heartPrefab;
        private List<GameObject> hearts = new();

        private void Start() {
            CreateHearts();
            if(healthComponent != null) {
                BindHealthComponent(healthComponent);
            }
        }
        
        private void CreateHearts() {
            for (int i = 0; i < healthComponent.maxHealth; i++) {
                GameObject heart = Instantiate(heartPrefab, heartsLayoutGroup.transform);
                hearts.Add(heart);
            }
        }
        
        private void BindHealthComponent(HealthComponent healthComponent) {
            healthComponent.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(healthComponent.health);
        }

        // TODO: for now we're just disabling the hearts, but this is just a placeholder and we should do something better
        private void UpdateHealthBar(float health) {
            // go backwards through the list of hearts
            for (int i = hearts.Count - 1; i >= 0; i--) {
                SetHeartEnabled(i, i + 1 <= health);
            }
        }

        private void SetHeartEnabled(int index, bool isEnabled) {
            // NOTE: this relies on a certain hierarchy of the heart prefab, which is not ideal
            // this assumes the first child is a full heart and the second child is an empty heart
            hearts[index].transform.GetChild(0).gameObject.SetActive(isEnabled);
        } 
    }
}