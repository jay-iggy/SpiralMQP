using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    public class GamblerAbility : MonoBehaviour
    {
        private GameObject player;
        private PlayerController pc;
        private AttackAbility primaryAbility;
        [SerializeField] Sound failSfx;


        void Start()
        {
            player = transform.parent.parent.gameObject;
            pc = player.GetComponent<PlayerController>();
            pc.weaponChanged.AddListener(SetAttackAbilities);

            SetAttackAbilities();
        }

        public void SetAttackAbilities()
        {
            if (pc.primaryAbility is AttackAbility)
            {
                primaryAbility = (AttackAbility)pc.primaryAbility;
                primaryAbility.onAttack.AddListener(Gamble);
                Gamble();
            }
        }

        public void Gamble()
        {
            if(Random.Range(0, 7) == 0)
            {
                primaryAbility.ModifyDamage(0, ModifyValue.MULT);
                failSfx.PlaySound();
            }
            else
            {
                primaryAbility.ModifyDamage(1.5f, ModifyValue.MULT);
            }
        }

    }
}
