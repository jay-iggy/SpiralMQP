//using Game.Scripts.Analytics;

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts {
    public class CustomStatsManager : MonoBehaviour {
        public CustomStats customStats;
        public CustomStats tempStats;
        public CustomStats baseStats;
        
        public UnityEvent onResetStats;
        
        public static CustomStatsManager instance;

        private void Awake() {
            if(instance == null) {
                instance = this;
                transform.parent = null;
                DontDestroyOnLoad(this);
            } else {
                Destroy(this);
            }
        }

        public void ResetCustomStats() {
            customStats = baseStats;
            tempStats = customStats;
            onResetStats.Invoke();
        }
        
        public void ClearTempStats() {
            tempStats = customStats;
        }
        
        public void ConfirmTempStats() {
            customStats = tempStats;
        }
    }
    
    [Serializable]
    public struct CustomStats {
        public int playerHealth;
        public float playerSpeed;
        public float playerAttackSpeed;
        public float enemyHealthMult;
        public float enemyAttackSpeedMult;
        
        public string[] ToArray() {
            return new string[] {playerHealth.ToString(), playerSpeed.ToString(), playerAttackSpeed.ToString(), enemyHealthMult.ToString(), enemyAttackSpeedMult.ToString() };
        }
    }
}