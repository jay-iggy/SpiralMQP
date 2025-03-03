using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class BerserkAbility : MonoBehaviour
    {
        private bool primaryActive = false;
        private bool offhandActive = false;
        private GameObject player;
        private HealthComponent hp;
        private PlayerController pc;
        private PranimDriver gfx;
        [SerializeField] Material berserkMaterial;
        [SerializeField] float damage;
        private AttackAbility primaryAbility;
        private AttackAbility offhandAbility;

        public Sound sfx;

        void Start()
        {
            player = transform.parent.parent.gameObject;
            gfx = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PranimDriver>();
            hp = player.GetComponent<HealthComponent>();
            pc = player.GetComponent<PlayerController>();
            hp.onHealthChanged.AddListener(SetActive);
            pc.weaponChanged.AddListener(SetAttackAbilities);
           
            SetAttackAbilities();
        }

        public void SetAttackAbilities()
        {
            if (pc.primaryAbility is AttackAbility)
            {
                primaryAbility = (AttackAbility)pc.primaryAbility;
            }
            if (pc.offhandAbility is AttackAbility)
            {
                offhandAbility = (AttackAbility)pc.offhandAbility;
            }

            SetActive(hp.health);
        }

        public void SetActive(float health)
        {
            if (health < hp.maxHealth / 2) //active
            {
                gfx.SetAllMaterialsToOneMat(berserkMaterial);
                
                if(!primaryActive && primaryAbility != null)
                {
                    primaryAbility.ModifyDamage(damage, ModifyValue.ADD);
                    primaryActive = true;
                }
                if (!offhandActive && offhandAbility != null)
                {
                    offhandAbility.ModifyDamage(damage, ModifyValue.ADD);
                    offhandActive = true;
                }
                
                PlaySound();
            }
            else //inactive
            {
                gfx.UpdateMaterialsToDefaults();
                if(primaryActive && primaryAbility != null)
                {
                    primaryAbility.ModifyDamage(-damage, ModifyValue.ADD);
                    primaryActive = false;
                    
                }
                if (offhandActive && offhandAbility != null)
                {
                    offhandAbility.ModifyDamage(damage, ModifyValue.ADD);
                    offhandActive = false;
                }

            }
        }

        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}
