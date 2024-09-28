using UnityEngine;

namespace Game.Scripts.Analytics {
    public class AnalyticsButtonEvents : MonoBehaviour {
        public void LoadNewPlayer() {
            AnalyticsManager.instance.IncrementPlayerNum();
            CustomStatsManager.instance.ResetCustomStats();
        }
    }
}