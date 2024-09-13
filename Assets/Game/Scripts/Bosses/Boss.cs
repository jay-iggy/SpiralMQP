using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Boss : MonoBehaviour
    {
        protected ICanAttack attacks;
        [SerializeField] float timeBetweenAttacks;
        private float attackTimer = 0;
        private int attackIndex = -1;
        private bool alive = true;

        void Start()
        {
            attacks = GetComponent<ICanAttack>();
        }

        void Update()
        {
            if (alive)
            {
                checkForAttack();
            }          
        }

        public void die()
        {
            alive = false;
        }

        protected void checkForAttack()
        {
            if (attackTimer >= timeBetweenAttacks)
            {               
                attackTimer = -1 * chooseAttack(); //timer will get to 0 as attack ends
            }

            attackTimer += Time.deltaTime;
        }

        protected float chooseAttack()
        {
            int attackToDo = attackIndex;
            while (attackToDo == attackIndex) { //don't do the same attack twice in a row
                attackToDo = Random.Range(0, attacks.numAttacks());
            }
            attackIndex = attackToDo;

            return attacks.Attack(attackIndex);
        }
    }

    

}
