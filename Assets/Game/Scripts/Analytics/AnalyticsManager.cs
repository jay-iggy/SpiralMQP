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
        }
        
        public AnalyticsData analyticsData;
        
        private void Start() {
            int playerNum = 0;
            if(PlayerPrefs.HasKey("PlayerNum")) {
                playerNum = PlayerPrefs.GetInt("PlayerNum") + 1;
            } 
            
            analyticsData = new AnalyticsData(Application.version, playerNum, new RunData(false));
        }
        
        public void SaveDataToCSV(AnalyticsData data) {
            CSVManager.AppendToReport(data.ToList());
        }
        
        public void IncrementPlayerNum() {
            analyticsData.playerNum++;
        }

        private void OnDestroy() {
            PlayerPrefs.SetInt("PlayerNum", analyticsData.playerNum);
        }
    }
}