using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Boss : MonoBehaviour {
        protected ICanAttack attacks; // needs more descriptive name
        [SerializeField] float attackDelay = 0.1f;
        private float attackTimer = 0;
        private int attackIndex = -1;
        private bool isAlive = true;

        void Start() {
            attacks = GetComponent<ICanAttack>();
        }

        void Update() {
            if (isAlive) {
                CheckForAttack();
            }          
        }

        public void Die() {
            isAlive = false;
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
                attackToDo = Random.Range(0, attacks.GetAttackCount());
            }
            attackIndex = attackToDo;

            return attacks.Attack(attackIndex);
        }
    }
}
