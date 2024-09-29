using UnityEngine;

namespace Game.Scripts.Analytics {
    public class AnalyticsEvents : MonoBehaviour {
        public void LoadNewPlayer() {
            AnalyticsManager.instance.IncrementPlayerNum();
            CustomStatsManager.instance.ResetCustomStats();
        }
    }
}