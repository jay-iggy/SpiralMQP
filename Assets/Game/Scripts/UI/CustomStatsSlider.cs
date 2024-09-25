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
        
        private Slider _slider;

        private void Awake() {
            _slider = GetComponentInChildren<Slider>();
        }

        private void Start() {
            CustomStatsManager.instance.tempStats = CustomStatsManager.instance.customStats;
            
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            
            minText.text = displayAsPercent ? $"{minValue*100}%" : $"{minValue}";
            maxText.text = displayAsPercent ? $"{maxValue*100}%" : $"{maxValue}";

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
                    _slider.value = CustomStatsManager.instance.customStats.playerHealth;
                    break;
                case CustomStatType.PlayerSpeed:
                    _slider.value = CustomStatsManager.instance.customStats.playerSpeed;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    _slider.value = CustomStatsManager.instance.customStats.playerAttackSpeed;
                    break;
                case CustomStatType.EnemyHealthMult:
                    _slider.value = CustomStatsManager.instance.customStats.enemyHealthMult;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    _slider.value = CustomStatsManager.instance.customStats.enemyAttackSpeedMult;
                    break;
            }
        }
        
        private void OnSliderValueChanged(float value) {
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    CustomStatsManager.instance.tempStats.playerHealth = (int)value;
                    break;
                case CustomStatType.PlayerSpeed:
                    CustomStatsManager.instance.tempStats.playerSpeed = value;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    CustomStatsManager.instance.tempStats.playerAttackSpeed = value;
                    break;
                case CustomStatType.EnemyHealthMult:
                    CustomStatsManager.instance.tempStats.enemyHealthMult = (int)value;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    CustomStatsManager.instance.tempStats.enemyAttackSpeedMult = value;
                    break;
            }
        }
        
        public void ResetSliderValue() {
            // set value to base stat
            switch (statType) {
                case CustomStatType.PlayerHealth:
                    _slider.value = CustomStatsManager.instance.baseStats.playerHealth;
                    break;
                case CustomStatType.PlayerSpeed:
                    _slider.value = CustomStatsManager.instance.baseStats.playerSpeed;
                    break;
                case CustomStatType.PlayerAttackSpeed:
                    _slider.value = CustomStatsManager.instance.baseStats.playerAttackSpeed;
                    break;
                case CustomStatType.EnemyHealthMult:
                    _slider.value = CustomStatsManager.instance.baseStats.enemyHealthMult;
                    break;
                case CustomStatType.EnemyAttackSpeedMult:
                    _slider.value = CustomStatsManager.instance.baseStats.enemyAttackSpeedMult;
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
    }
    
    public enum CustomStatType {
        PlayerHealth,
        PlayerSpeed,
        PlayerAttackSpeed,
        EnemyHealthMult,
        EnemyAttackSpeedMult
    }
}