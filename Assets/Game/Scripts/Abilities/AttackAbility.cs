using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public enum ModifyValue
    {
        SET,
        ADD,
        MULT
    }
    public abstract class AttackAbility : Ability
    {
        public float baseDamage;
        private float damageMultiplier = 1;
        private float damagePlus = 0;
        public void ModifyDamage(float damage, ModifyValue modify)
        {
            switch(modify)
            {
                case ModifyValue.SET:
                    baseDamage = damage;
                    break;
                case ModifyValue.ADD:
                    damagePlus += damage;
                    break;
                case ModifyValue.MULT:
                    damageMultiplier = damage;
                    break;
            }
        }

        public float CalculateDamage()
        {
            return baseDamage * damageMultiplier + damagePlus;
        }

        public UnityEvent onAttack;
    }
}
