using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Scripts.Analytics {
    public class AnalyticsManager : MonoBehaviour {
        public static AnalyticsManager instance;
        
        private const string SEPARATOR = ",";
        private const string FILE_NAME = "spiral-analytics.csv";
        private const string DIR_NAME = "Analytics";
        private static string[] reportHeaders = {
            "Version",
            "Player #",
            "Player Won"
        };
        private static string[] bossHeaders = {
            "Player Damage Taken",
            "Player Time Taken"
        };
        
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
            analyticsData = new AnalyticsData(Application.version, playerNum, new RunData(false), new CustomStats());
            CombatManager.instance.onGameStart.AddListener(OnGameStart);
        }
        private void OnEnable() {
            if(CombatManager.instance == null) {
                return;
            }
            CombatManager.instance.onGameStart.AddListener(OnGameStart);
        }

        private void OnDisable() {
            CombatManager.instance.onGameStart.RemoveListener(OnGameStart);
        }

        private void OnGameStart() {
            VerifyFile();
            
            CombatManager.instance.onBossDefeated.AddListener(TrackBossAnalytics);
            prevPlayerHealth = CombatManager.instance.playerHealth.maxHealth;
            analyticsData.customStats = CustomStatsManager.instance.customStats;
            
            isTimerRunning = true;
            bossAnalyticsTimer = 0;
            
            analyticsData.runData.bossData = new List<string>();
        }

        private void VerifyFile() {
            VerifyDirectory();
            string file = GetFilePath();
            if (!File.Exists(file)) {
                CreateFile();
            }
        }
        private void VerifyDirectory() {
            string dir = GetDirectoryPath();
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        private void CreateFile() {
            using (StreamWriter sw = File.CreateText(GetFilePath())) {
                string headerLine = string.Join(SEPARATOR, reportHeaders);
                
                string customStatsHeader = string.Join(SEPARATOR, CustomStatsManager.customStatsHeaders);
                
                string bossHeader = "";
                EnemyData enemyData = CombatManager.instance.currentEnemyData;
                while (enemyData != null) {
                    string enemyName = enemyData.enemyName;
                    for (int i = 0; i < bossHeaders.Length; i++) {
                        bossHeader += $"{enemyName}: {bossHeaders[i]}";
                        if (i < bossHeaders.Length) {
                            bossHeader += SEPARATOR;
                        }
                    }
                    if(enemyData.nextEnemies.Count > 0) {
                        enemyData = enemyData.nextEnemies[0];
                    }
                    else {
                        enemyData = null;
                    }
                }
                
                sw.WriteLine($"Time,{headerLine},{customStatsHeader},{bossHeader}");
            }
        }
        
        public void SaveDataToCSV() {
            string dataString = $"{GetTimestamp()}{SEPARATOR}";
            dataString += string.Join(SEPARATOR, analyticsData.ToList());
            File.AppendAllText(GetFilePath(), dataString + "\n");
        }
        
        public void IncrementPlayerNum() {
            analyticsData.playerNum++;
        }

        private void OnDestroy() {
            PlayerPrefs.SetInt("PlayerNum", analyticsData.playerNum);
        }
        
        
        static string GetDirectoryPath() {
            #if UNITY_EDITOR
                return $"{Application.dataPath}/{DIR_NAME}";
            #else
                string parent = System.IO.Directory.GetParent(Application.dataPath).FullName;
                return $"{parent}/{DIR_NAME}";
            #endif
        }

        static string GetFilePath() {
            return $"{GetDirectoryPath()}/{FILE_NAME}";
        }
        static string GetTimestamp() {
            return System.DateTime.Now.ToString();
        }
        
        private float bossAnalyticsTimer = 0;
        private bool isTimerRunning = false;
        private float prevPlayerHealth = 0;
        public void TrackBossAnalytics() {
            float playerDamageTaken = Mathf.Clamp(prevPlayerHealth - CombatManager.instance.playerHealth.health, 0, CombatManager.instance.playerHealth.maxHealth);
            string timeTaken = $"{(bossAnalyticsTimer / 60).ToString("00")}:{(bossAnalyticsTimer % 60).ToString("00")}";
            
            
            string bossData = $"{playerDamageTaken}, {timeTaken}";
            Debug.Log("Boss defeated, player damage taken: " + playerDamageTaken + ", time taken: " + timeTaken);
            analyticsData.runData.bossData.Add(bossData);
            
            // maybe stored boss health, to see how far they got when they lose to a boss
            
            
            bossAnalyticsTimer = 0;
            prevPlayerHealth = CombatManager.instance.playerHealth.maxHealth;
        }

        private void Update() {
            if(isTimerRunning) {
                bossAnalyticsTimer += Time.deltaTime;
            }
        }
    }
}