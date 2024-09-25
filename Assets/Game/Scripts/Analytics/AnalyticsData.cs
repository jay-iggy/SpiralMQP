using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Analytics {
    public struct AnalyticsData {
        public string version;
        public string playerNum;
        
        RunData runData;
        
        public AnalyticsData(string version, string playerNum) {
            this.version = version;
            this.playerNum = playerNum;
            runData = new RunData();
        }
        
        public List<string> ToArray() {
            List<string> strings = new List<string> { version, playerNum };
            strings.AddRange(runData.ToArray());
            return strings;
        }
    }

    public struct RunData {
        public bool isWin;
        SurveyData surveyData;
        
        public RunData(bool isWin) {
            surveyData = new SurveyData();
            this.isWin = isWin;
        }
        
        public List<string> ToArray() {
            return new List<string> {isWin.ToString()};
        }
    }
    
    
    public struct SurveyData {
        public string[] ToArray() {
            return new string[] {};
        }
    }
}