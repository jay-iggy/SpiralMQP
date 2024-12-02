using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities {
    public class BerserkAbility : MonoBehaviour
    {
        private bool active = false;
        private GameObject player;
        private HealthComponent hp;
        private PlayerController pc;
        private PranimDriver gfx;
        [SerializeField] Material berserkMaterial;
        [SerializeField] int damage;
        private AttackAbility attackAbility;

        public Sound sfx;

        void Start()
        {
            player = transform.parent.parent.gameObject;
            gfx = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PranimDriver>();
            hp = player.GetComponent<HealthComponent>();
            pc = player.GetComponent<PlayerController>();
            hp.onHealthChanged.AddListener(SetActive);

            if(pc.primaryAbility is AttackAbility)
            {
                attackAbility = (AttackAbility)pc.primaryAbility;
            }

            SetActive(hp.health);
        }

        public void SetActive(float health)
        {
            if (health < hp.maxHealth / 2) //active
            {
                gfx.SetAllMaterialsToOneMat(berserkMaterial);
                // gfx.material = berserkMaterial;
                if(!active && attackAbility != null)
                {
                    attackAbility.ModifyDamage(damage);
                }
                active = true;
                PlaySound();
            }
            else //inactive
            {
                gfx.UpdateMaterialsToDefaults();
                if(active && attackAbility != null)
                {
                    attackAbility.ModifyDamage(-damage);
                }
                active = false;
            }
        }

        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}
