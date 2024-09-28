using UnityEngine;

namespace Game.Scripts.Player {
    public class CustomStatsButtonEvents : MonoBehaviour {
        public void ResetStats() {
            CustomStatsManager.instance.ResetCustomStats();
        }
        public void ConfirmStats() {
            CustomStatsManager.instance.ConfirmTempStats();
        }
    }
}