using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Boss : MonoBehaviour {
        protected ICanAttack attackList; 
        [SerializeField] float attackDelay = 0.1f;
        private float attackTimer = 0;
        private int attackIndex = -1;
        private bool isAlive = true;
        [SerializeField] int bossIndex; //used to match bosses to stickers

        void Start() {
            attackList = GetComponent<ICanAttack>();
            StickerManager.instance.hitless = true; //reset hitless tracker for each boss
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
                StickerManager.instance.showSticker(bossIndex);
            }          
        }

        protected void CheckForAttack() {
            if (attackTimer >= attackDelay) {               
                attackTimer = -1 * ChooseAttack(); //timer will get to 0 as attack ends
            }

            attackTimer += Time.deltaTime;
        }

        protected float ChooseAttack() {
            int attackToDo = attackIndex;
            while (attackToDo == attackIndex) { //don't do the same attack twice in a row
                attackToDo = Random.Range(0, attackList.GetAttackCount());
            }
            attackIndex = attackToDo;

            return attackList.Attack(attackIndex);
        }
    }
}
