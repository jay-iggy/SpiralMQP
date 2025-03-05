using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(HealthComponent))]
    public class Boss : MonoBehaviour {
        protected ICanAttack attackList; 
        [SerializeField] public float attackDelay = 0.1f;
        private float attackTimer = 0;
        private int attackIndex = -1;
        public bool isAlive = true;
        private bool waitForAttack = false;
        [SerializeField] protected List<ItemPickup> unlockedItems;
        [SerializeField] protected Sound onHitSfx;


        void Start() {
            attackList = GetComponent<ICanAttack>();
            
            HealthComponent healthComponent = GetComponent<HealthComponent>();
            healthComponent.maxHealth *= CustomStatsManager.instance.customStats.enemyHealthMult;
            healthComponent.SetHealth(healthComponent.maxHealth);
            
            healthComponent.onTakeDamage.AddListener(OnHit);
            
            attackDelay *= CustomStatsManager.instance.customStats.enemyAttackSpeedMult;
        }

        void Update() {
            if (isAlive) {
                CheckForAttack();
            }          
        }

        public virtual void Die() {
            isAlive = false;
            
            int bossIndex = CombatManager.instance.currentEnemyData.bossIndex;
            string bossKey = "boss" + bossIndex + "defeated";

            CombatManager.instance.BossWasDefeated();
            CombatManager.instance.DestroyBullets();

            if (PlayerPrefs.GetInt(bossKey, 0) == 0)
            {
                PlayerPrefs.SetInt(bossKey, 1);
            }
            

            if (StickerManager.instance != null) {
                if (StickerManager.instance.hitless)
                {
                    PlayerPrefs.SetInt(bossKey, 2); //mark as defeated hitless
                }

                StickerManager.instance.ShowSticker(bossIndex);
            }

            
            Destroy(gameObject);
            
        }

        void OnHit() {
            if(onHitSfx != null) {
                onHitSfx.PlaySound();
            }
        }

        protected void CheckForAttack() {
            if (waitForAttack) return;

            if (attackTimer >= attackDelay) {               
                attackTimer = -1 * ChooseAttack(); //timer will get to 0 as attack ends
            }

            attackTimer += Time.deltaTime;
        }

        protected float ChooseAttack() {
            if (attackList == null) return 0;

            if(attackList.GetAttackCount() <= 1) {
                return attackList.Attack(0);
            }

            int attackToDo = attackIndex;
            while (attackToDo == attackIndex) { //don't do the same attack twice in a row
                attackToDo = Random.Range(0, attackList.GetAttackCount());
            }
            attackIndex = attackToDo;
            float attackLength = attackList.Attack(attackIndex);
            if(attackLength == -1) {
                waitForAttack = true;
                return 0;
            }
            else {
                return attackLength;
            }          
        }

        public void DoneWithAttack() {
            waitForAttack = false;
        }
    }
}
