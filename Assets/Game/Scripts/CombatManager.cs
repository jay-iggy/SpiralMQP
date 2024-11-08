using System;
using System.Collections;
using Game.Scripts.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
   
    


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
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart() {
            yield return null;
            onGameStart.Invoke();
        }


        [SerializeField] private EnemyData initialBoss;
        [SerializeField] private float bossSpawnDelay = 2f;
        public Boss currentBoss { get; private set; }
        public EnemyData currentEnemyData { get; private set; }

        public UnityEvent onGameStart = new();
        public UnityEvent onBossDefeated = new();
        public UnityEvent onFinalBossDefeated = new();
        
        public UnityEvent onPlayerWin = new();
        public UnityEvent onPlayerLose = new();
        
        public HealthComponent playerHealth;

        public AudioManager AudioCON;

        public void DestroyBullets()
        {
            //destroy all enemy bullets
            // this is temporary until we have a better way to handle this
            foreach (Projectile p in FindObjectsOfType<Projectile>())
            {
                Destroy(p.gameObject);
            }
        }

        public void TransitionToNextBoss() {
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

            AudioCON.PlaySFX("boss_transition");

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
            if(AnalyticsManager.instance != null) {
                AnalyticsManager.instance.analyticsData.runData.isWin = true;
                AnalyticsManager.instance.SaveDataToCSV();
            }
            
            onPlayerWin.Invoke();
        }
        public void OnPlayerLose() {
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.analyticsData.runData.isWin = false;
                AnalyticsManager.instance.TrackBossAnalytics();
                AnalyticsManager.instance.SaveDataToCSV();
            }
            
            onPlayerLose.Invoke();
        }
    }
}