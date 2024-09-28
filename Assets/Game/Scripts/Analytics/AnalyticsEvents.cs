using UnityEngine;

namespace Game.Scripts.Analytics {
    public class AnalyticsEvents : MonoBehaviour {
        public void LoadNewPlayer() {
            AnalyticsManager.instance.IncrementPlayerNum();
            CustomStatsManager.instance.ResetCustomStats();
        }

        public void TempLose() {
            RunData runData = new RunData(false);
            
            AnalyticsManager.instance.analyticsData.runData = runData;
            AnalyticsManager.instance.SaveDataToCSV(AnalyticsManager.instance.analyticsData);
        }
    }
}