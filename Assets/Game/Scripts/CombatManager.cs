using System;
using System.Collections;
using Game.Scripts.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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

            onGameStart = new();
        }

        private void Start() {
            StartCoroutine(SpawnBoss(initialBoss,0));
            onGameStart.Invoke();
        }

        [SerializeField] private EnemyData initialBoss;
        [SerializeField] private float bossSpawnDelay = 2f;
        public Boss currentBoss { get; private set; }
        public EnemyData currentEnemyData { get; private set; }

        public static UnityEvent onGameStart = new();
        public UnityEvent onBossDefeated = new();
        public UnityEvent onFinalBossDefeated = new();
        
        public HealthComponent playerHealth;
        
        

        public void TransitionToNextBoss() {
            Destroy(currentBoss.gameObject);
            onBossDefeated.Invoke();
            
            
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
            
            //destroy all enemy bullets
            // this is temporary until we have a better way to handle this
            foreach (Projectile p in FindObjectsOfType<Projectile>()) {
                Destroy(p.gameObject);
            }
            
            // TODO: Add transition effects
        }
        private IEnumerator SpawnBoss(EnemyData enemyData, float delay) {
            yield return new WaitForSeconds(delay);
            BossTransitionManager.instance.SpawnBoss(enemyData, out Boss b);
            currentBoss = b;
            currentEnemyData = enemyData;
        }

        public void OnPlayerWin() {
            AnalyticsManager.instance.analyticsData.runData.isWin = true;
            AnalyticsManager.instance.SaveDataToCSV();
        }
        public void OnPlayerLose() {
            AnalyticsManager.instance.analyticsData.runData.isWin = false;
            AnalyticsManager.instance.SaveDataToCSV();
        }
    }
}