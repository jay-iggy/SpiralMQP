using System;
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
        
        protected int playerNum = 0;
        protected string version = Application.version;
        
        public void IncrementPlayerNum() {
            playerNum++;
        }
    }
}