using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Analytics {
    public struct AnalyticsData {
        public string version;
        public int playerNum;
        
        public RunData runData;
        public CustomStats customStats;
        
        
        public AnalyticsData(string version, int playerNum, RunData runData, CustomStats customStats){
            this.version = version;
            this.playerNum = playerNum;
            this.runData = runData;
            this.customStats = customStats;
        }
        
        public List<string> ToList() {
            List<string> strings = new List<string> { version, playerNum.ToString() };
            strings.Add(runData.isWin.ToString());
            strings.AddRange(customStats.ToArray());
            strings.AddRange(runData.ToList());
            return strings;
        }
    }

    public struct RunData {
        public bool isWin;
        public List<string> bossData;
        //SurveyData surveyData;
        
        public RunData(bool isWin) {
            //surveyData = new SurveyData();
            this.isWin = isWin;
            bossData = new List<string>();
        }
        
        public List<string> ToList() {
            List<string> strings = new List<string>();
            strings.AddRange(bossData);
            //strings.AddRange(surveyData.ToList());
            return strings;
        }
    }
    
    
    public struct SurveyData {
        public string[] ToList() {
            return new string[] {};
        }
    }
}