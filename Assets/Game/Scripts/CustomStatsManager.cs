//using Game.Scripts.Analytics;
using UnityEngine;

namespace Game.Scripts {
    public class CustomStatsManager : MonoBehaviour {
        
        
        public void ResetCustomStats() {
            //AnalyticsManager.instance.IncrementPlayerNum();
        }
    }
    
    public struct CustomStats {
        public int playerHealth;
        public float playerSpeed;
        public float playerAttackSpeed;
        public int enemyHealthMult;
        public float enemyAttackSpeedMult;
        
        public string[] ToArray() {
            return new string[] {playerHealth.ToString(), playerSpeed.ToString(), playerAttackSpeed.ToString(), enemyHealthMult.ToString(), enemyAttackSpeedMult.ToString() };
        }
    }
}