using System;
using System.Collections;
using Game.Scripts.Analytics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Scripts {
    public class CombatManager : MonoBehaviour {
        public static CombatManager instance;

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            StartCoroutine(SpawnBoss(initialBoss,0));
        }

        [SerializeField] private EnemyData initialBoss;
        [SerializeField] private float bossSpawnDelay = 2f;
        public Boss currentBoss { get; private set; }
        public EnemyData currentEnemyData { get; private set; }

        public UnityEvent onFinalBossDefeated = new();

        public void TransitionToNextBoss() {
            Destroy(currentBoss.gameObject);
            
            
            if (currentEnemyData == null) {
                Debug.LogError("No current enemy data to transition from");
            }
            if (currentEnemyData.nextEnemies.Count == 0) {
                OnPlayerWin();
                onFinalBossDefeated.Invoke();
                return;
            }
            EnemyData nextEnemyData = currentEnemyData.nextEnemies[Random.Range(0, currentEnemyData.nextEnemies.Count)];

            StickerManager.instance.hitless = true; //reset hitless tracker for each boss

            StartCoroutine(SpawnBoss(nextEnemyData, bossSpawnDelay));
            
            // TODO: Add transition effects
        }
        private IEnumerator SpawnBoss(EnemyData enemyData, float delay) {
            yield return new WaitForSeconds(delay);
            BossTransitionManager.instance.SpawnBoss(enemyData, out Boss b);
            currentBoss = b;
            currentEnemyData = enemyData;
        }

        public void OnPlayerWin() {
            RunData runData = new RunData(true);
            
            AnalyticsManager.instance.analyticsData.runData = runData;
            AnalyticsManager.instance.SaveDataToCSV(AnalyticsManager.instance.analyticsData);
        }
        public void OnPlayerLose() {
            RunData runData = new RunData(false);
            
            AnalyticsManager.instance.analyticsData.runData = runData;
            AnalyticsManager.instance.SaveDataToCSV(AnalyticsManager.instance.analyticsData);
        }
    }
}