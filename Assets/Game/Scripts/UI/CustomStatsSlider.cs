using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts {
    public class CustomStatsSlider : MonoBehaviour {
        [SerializeField] CustomStatType statType;
        [SerializeField] float minValue = 0;
        [SerializeField] float maxValue = 100;
        [SerializeField] bool displayAsPercent = false;
        [Header("References")]
        [SerializeField] TextMeshProUGUI minText;
        [SerializeField] TextMeshProUGUI maxText;
        [SerializeField] RectTransform sliderFill;
        [SerializeField] Image defaultValueMarker;
        
        private Slider slider;

        private void Awake() {
            slider = GetComponentInChildren<Slider>();
        }

        private void Start() {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            
            minText.text = minValue.ToString() + (displayAsPercent ? "%" : "");
            maxText.text = maxValue.ToString() + (displayAsPercent ? "%" : "");

            SetInitialSliderValue();
            SetDefaultMarkerPosition();
        }

        private void OnEnable() {
            CustomStatsManager.instance.onResetStats.AddListener(ResetSliderValue);
        }

        private void OnDisable() {
            CustomStatsManager.instance.onResetStats.RemoveListener(ResetSliderValue);
        }

        private void SetInitialSliderValue() {
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    slider.value = CustomStatsManager.instance.customStats.playerHealth;
                    break;
                case CustomStatType.PlayerSpeed:
                    slider.value = CustomStatsManager.instance.customStats.playerSpeed;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    slider.value = CustomStatsManager.instance.customStats.playerAttackSpeed;
                    break;
                case CustomStatType.EnemyHealthMult:
                    slider.value = CustomStatsManager.instance.customStats.enemyHealthMult;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    slider.value = CustomStatsManager.instance.customStats.enemyAttackSpeedMult;
                    break;
            }
        }
        
        private void OnSliderValueChanged(float value) {
            if (displayAsPercent) {
                value /= 100;
            }
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    SetPlayerHealth(value);
                    break;
                case CustomStatType.PlayerSpeed:
                    SetPlayerSpeed(value);
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    SetPlayerAttackSpeed(value);
                    break;
                case CustomStatType.EnemyHealthMult:
                    SetEnemyHealthMult(value);
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    SetEnemyAttackSpeedMult(value);
                    break;
            }
        }
        
        public void ResetSliderValue() {
            // set value to base stat
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    slider.value = CustomStatsManager.instance.baseStats.playerHealth;
                    break;
                case CustomStatType.PlayerSpeed:
                    slider.value = CustomStatsManager.instance.baseStats.playerSpeed;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    slider.value = CustomStatsManager.instance.baseStats.playerAttackSpeed;
                    break;
                case CustomStatType.EnemyHealthMult:
                    slider.value = CustomStatsManager.instance.baseStats.enemyHealthMult;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    slider.value = CustomStatsManager.instance.baseStats.enemyAttackSpeedMult;
                    break;
            }
        }

        private void SetDefaultMarkerPosition() {
            // get base stat value
            float baseStatValue = 0;
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    baseStatValue = CustomStatsManager.instance.baseStats.playerHealth;
                    break;
                case CustomStatType.PlayerSpeed:
                    baseStatValue = CustomStatsManager.instance.baseStats.playerSpeed;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    baseStatValue = CustomStatsManager.instance.baseStats.playerAttackSpeed;
                    break;
                case CustomStatType.EnemyHealthMult:
                    baseStatValue = CustomStatsManager.instance.baseStats.enemyHealthMult;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    baseStatValue = CustomStatsManager.instance.baseStats.enemyAttackSpeedMult;
                    break;
            }
            
            // find the position of the base stat value on the slider
            float fillWidth = sliderFill.rect.width;
            float fillPercent = (baseStatValue - minValue) / (maxValue - minValue);
            float fillX = fillWidth * fillPercent - (fillWidth / 2);
            defaultValueMarker.rectTransform.anchoredPosition = new Vector2(fillX, 0);
        }

        private void SetPlayerHealth (float value) {
            CustomStatsManager.instance.tempStats.playerHealth = (int)value;
        }
        private void SetPlayerSpeed (float value) {
            CustomStatsManager.instance.tempStats.playerSpeed = value;
        }
        private void SetPlayerAttackSpeed (float value) {
            CustomStatsManager.instance.tempStats.playerAttackSpeed = value;
        }
        private void SetEnemyHealthMult (float value) {
            CustomStatsManager.instance.tempStats.enemyHealthMult = (int)value;
        }
        private void SetEnemyAttackSpeedMult (float value) {
            CustomStatsManager.instance.tempStats.enemyAttackSpeedMult = value;
        }
    }
    
    public enum CustomStatType {
        PlayerHealth,
        PlayerSpeed,
        PlayerAttackSpeed,
        EnemyHealthMult,
        EnemyAttackSpeedMult
    }
}