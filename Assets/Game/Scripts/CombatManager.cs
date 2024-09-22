using System;
using UnityEngine;
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
            SpawnBoss(initialBoss);
        }

        [SerializeField] private EnemyData initialBoss;
        public Boss currentBoss { get; private set; }
        public EnemyData currentEnemyData { get; private set; }

        public void TransitionToNextBoss() {
            Destroy(currentBoss.gameObject);
            
            
            if (currentEnemyData == null) {
                Debug.LogError("No current enemy data to transition from");
            }
            if (currentEnemyData.nextEnemies.Count == 0) {
                Debug.LogError("No next enemies to transition to");
            }
            EnemyData nextEnemyData = currentEnemyData.nextEnemies[Random.Range(0, currentEnemyData.nextEnemies.Count)];
            SpawnBoss(nextEnemyData);
            
            // TODO: Add transition effects
        }
        private void SpawnBoss(EnemyData enemyData) {
            BossTransitionManager.instance.SpawnBoss(enemyData, out Boss b);
            currentBoss = b;
            currentEnemyData = enemyData;
        }
    }
}