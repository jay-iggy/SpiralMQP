using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class BerserkAbility : PassiveAbility
    {
        private bool active = false;
        private GameObject player;
        private HealthComponent hp;
        private PlayerController pc;
        private MeshRenderer gfx;
        [SerializeField] Material berserkMaterial;
        private Material normalMaterial;
        private AttackAbility attackAbility;
        void Start()
        {
            player = transform.parent.parent.gameObject;
            gfx = player.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
            normalMaterial = gfx.material;
            hp = player.GetComponent<HealthComponent>();
            pc = player.GetComponent<PlayerController>();
            hp.onHealthChanged.AddListener(SetActive);

            if(pc.primaryAbility is AttackAbility)
            {
                attackAbility = (AttackAbility)pc.primaryAbility;
            }           
        }

        public void SetActive(float health)
        {
            if (health < hp.maxHealth / 2) //active
            {
                gfx.material = berserkMaterial;
                if(!active && attackAbility != null)
                {
                    attackAbility.ModifyDamage(1);
                }
                active = true;
            }
            else //inactive
            {
                gfx.material = normalMaterial;
                if(active && attackAbility != null)
                {
                    attackAbility.ModifyDamage(-1);
                }
                active = false;
            }
        }
    }
}
