using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Scripts {
    public class CombatManager : MonoBehaviour {
        public static CombatManager instance;

        [SerializeField] private EnemyData initialBoss;
        [SerializeField] private EnemyData tutorialBoss;
        public List<BossTier> bossTiers;
        public List< EnemyData> finalBossPhases = new List<EnemyData>();
        [SerializeField] List<int> bossDifficultyOrder = new List<int>{ 0, 0, 1, 2, 2 };
        
        private int bossNumber = 0;
        
        private EnemyData nextBoss = null;
        [SerializeField] private float bossSpawnDelay = 2f;
        public Boss currentBoss { get; private set; }
        public EnemyData currentEnemyData { get; private set; }

        public UnityEvent onGameStart = new();
        public UnityEvent onBossSpawned = new ();
        public UnityEvent onBossDefeated = new();
        public UnityEvent onTransitionToFinalBoss = new();
        public UnityEvent onFinalBossPhaseChange = new();
        public UnityEvent onFinalBossDefeated = new();
        
        public UnityEvent onPlayerWin = new();
        public UnityEvent onPlayerLose = new();
        
        public HealthComponent playerHealth;

        public AudioManager AudioCON;
        
        private BossType bossType = BossType.NORMAL;
        public EnemyData nextEnemyData = null;
        
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
            playerHealth = FindObjectOfType<PlayerController>().GetComponent<HealthComponent>();
        }

        private IEnumerator LateStart() {
            yield return null;
            onGameStart.Invoke();
        }
        
        public void DestroyBullets() {
            //destroy all enemy bullets
            // this is temporary until we have a better way to handle this
            foreach (Projectile p in FindObjectsOfType<Projectile>()) {
                Destroy(p.gameObject);
            }
        }

        public void BossWasDefeated() {
            nextEnemyData = null;
            if (bossType == BossType.NORMAL) {
                if(bossNumber >= bossDifficultyOrder.Count) {
                    bossType = BossType.FINAL;
                    onTransitionToFinalBoss.Invoke();
                }
                else {
                    if(bossNumber<1 && tutorialBoss != null && PlayerPrefs.GetInt("boss" + 1 + "defeated", 0) == 0) {
                        nextEnemyData = tutorialBoss;
                        bossTiers[0].bosses.Remove(tutorialBoss);
                    }
                    else {
                        int tierNum = bossDifficultyOrder[bossNumber];
                        BossTier tier = bossTiers[tierNum];
                        nextEnemyData = GetRandomBoss(tier.bosses);
                    }
                    onBossDefeated.Invoke();
                }
            }
            if(bossType == BossType.FINAL) {
                int finalBossPhase = bossNumber - bossDifficultyOrder.Count;
                if (finalBossPhase >= finalBossPhases.Count) {
                    OnPlayerWin();
                    onFinalBossDefeated.Invoke();
                    return;
                }
                
                nextEnemyData = finalBossPhases[finalBossPhase];

                if (finalBossPhase > 0) { // skip items
                    onFinalBossPhaseChange.Invoke();
                    TransitionToNextBoss();
                    if(playerHealth != null) {
                        playerHealth.SetHealth(playerHealth.maxHealth);
                    }
                }
            }
            
            bossNumber++;
        }
        

        public void TransitionToNextBoss() {
            if (currentEnemyData == null) {
                Debug.LogError("No current enemy data to transition from");
            }
            
            StickerManager.instance.hitless = true; //reset hitless tracker for each boss

            if(AudioCON != null)
            AudioCON.PlaySFX("boss_transition");

            currentBoss = null;
            StartCoroutine(SpawnBoss(nextEnemyData, bossSpawnDelay));
            
            // TODO: Add transition effects
        }

        private EnemyData GetRandomBoss(List<EnemyData> enemyData)
        {
            int r = Random.Range(0, enemyData.Count);
            EnemyData output = enemyData[r];
            enemyData.RemoveAt(r);
            return output;
        }
        private IEnumerator SpawnBoss(EnemyData enemyData, float delay) {
            yield return new WaitForSeconds(delay);
            BossTransitionManager.instance.SpawnBoss(enemyData, out Boss b);
            currentBoss = b;
            currentEnemyData = enemyData;
            onBossSpawned.Invoke();
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

[Serializable]
public class BossTier
{
    public List<EnemyData> bosses;
}

public enum BossType {
    NORMAL,
    FINAL
}