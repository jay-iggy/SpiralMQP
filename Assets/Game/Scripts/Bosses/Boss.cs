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
        private bool waitForAttack = false;
        private bool hasBeenDefeated = false;
        [SerializeField] int bossIndex; //used to match bosses to stickers
        [SerializeField] List<ItemPickup> unlockedItems;
        [SerializeField] ItemRarity minItemRarity = ItemRarity.COMMON;
        string bossKey;


        void Start() {
            bossKey = "boss"+bossIndex+"defeated";
            PlayerPrefs.SetInt(bossKey, 0); //remove this when item saving is fixed
            if (PlayerPrefs.GetInt(bossKey, 0) == 1)
            {
                hasBeenDefeated = true;
            }

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
            PlayerPrefs.SetInt(bossKey, 1);

            if(StickerManager.instance != null)
            {
                StickerManager.instance.ShowSticker(bossIndex);
            }

            CombatManager.instance.BossWasDefeated();

            if(PickupManager.instance != null)
            {
                PickupManager.instance.DropItems(minItemRarity);

                if (!hasBeenDefeated && unlockedItems.Count>0)
                {
                    ReleaseItems();
                }
            }
            else //if pickup manager exists, it will handle boss transition
            {
                CombatManager.instance.TransitionToNextBoss();
            }

            CombatManager.instance.DestroyBullets();
            Destroy(gameObject);
            
        }

        private void ReleaseItems()
        {
            PickupManager.instance.pickups.AddRange(unlockedItems);
            PickupManager.instance.permanentItemPool.AddRange(unlockedItems);
            string allItems = PickupManager.instance.SerializeItemList();
            Debug.Log(allItems);
            PlayerPrefs.SetString("itemPool", allItems);
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

            if(attackList.GetAttackCount() <= 1)
            {
                
                return attackList.Attack(0);
            }

            int attackToDo = attackIndex;
            while (attackToDo == attackIndex) { //don't do the same attack twice in a row
                attackToDo = Random.Range(0, attackList.GetAttackCount());
            }
            attackIndex = attackToDo;
            float attackLength = attackList.Attack(attackIndex);
            if(attackLength == -1)
            {
                waitForAttack = true;
                return 0;
            }
            else
            {
                return attackLength;
            }          
        }

        public void DoneWithAttack()
        {
            waitForAttack = false;
        }
    }
}
