using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Analytics {
    public class AnalyticsManager : MonoBehaviour {
        public static AnalyticsManager instance;

        private void Awake() {
            if (instance == null) {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
                instance = this;
            } else {
                Destroy(gameObject);
            }
            version = Application.version;
        }

        protected int playerNum = 0;
        protected string version;
        
        private void Start() {
            CSVManager.CreateReport();
            
            SaveDataToCSV(new AnalyticsData(version, playerNum.ToString()));
        }
        
        public void SaveDataToCSV(AnalyticsData data) {
            CSVManager.AppendToReport(data.ToArray());
        }
        
        public void IncrementPlayerNum() {
            playerNum++;
        }

    }
}