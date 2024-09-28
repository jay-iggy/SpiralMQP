using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(HealthComponent))]
    public class Boss : MonoBehaviour {
        protected ICanAttack attackList; 
        [SerializeField] float attackDelay = 0.1f;
        private float attackTimer = 0;
        private int attackIndex = -1;
        private bool isAlive = true;
        [SerializeField] int bossIndex; //used to match bosses to stickers

        void Start() {
            attackList = GetComponent<ICanAttack>();
            
            HealthComponent healthComponent = GetComponent<HealthComponent>();
            healthComponent.maxHealth *= CustomStatsManager.instance.customStats.enemyHealthMult;
            healthComponent.SetHealth(healthComponent.maxHealth);
            
            attackDelay *= CustomStatsManager.instance.customStats.enemyAttackSpeedMult;
        }

        void Update() {
            if (isAlive) {
                CheckForAttack();
            }          
        }

        public void Die() {
            isAlive = false;
            if(StickerManager.instance != null)
            {
                StickerManager.instance.ShowSticker(bossIndex);
            }          
            CombatManager.instance.TransitionToNextBoss();
        }

        protected void CheckForAttack() {
            if (attackTimer >= attackDelay) {               
                attackTimer = -1 * ChooseAttack(); //timer will get to 0 as attack ends
            }

            attackTimer += Time.deltaTime;
        }

        protected float ChooseAttack() {
            if (attackList == null) return 0;

            int attackToDo = attackIndex;
            while (attackToDo == attackIndex) { //don't do the same attack twice in a row
                attackToDo = Random.Range(0, attackList.GetAttackCount());
            }
            attackIndex = attackToDo;

            return attackList.Attack(attackIndex);
        }
    }
}
