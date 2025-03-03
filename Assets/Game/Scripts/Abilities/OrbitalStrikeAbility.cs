using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Player.Abilities
{
    public class OrbitalStrikeAbility : Ability
    {


        [SerializeField] GameObject beam;
        private GameObject currentBeam;
        private float selfDestructTimer = -1;

        public override void OnAbilityEquipped() {
            _player.GetReticle().enabled = false;
        }

        public override void AbilityPressed()
        {
            if (CombatManager.instance.currentBoss == null || selfDestructTimer != -1) return;

            GameObject target = CombatManager.instance.currentBoss.gameObject;
            HealthComponent health = target.GetComponent<HealthComponent>();
            currentBeam = Instantiate(beam, target.transform.position, Quaternion.identity);
            health.TakeDamage(99.5f, true);
            selfDestructTimer = 0.5f;
            ScreenShake.instance.StartShake(1, 1f);
        }

        private void Update()
        {
            if (selfDestructTimer == -1) return;

            selfDestructTimer -= Time.deltaTime;
            if (selfDestructTimer <= 0)
            {
                Destroy(currentBeam);
                GameObject.FindGameObjectWithTag(TagManager.Player).GetComponent<PlayerController>().RemoveAbility(this);
            }
        }

        public override void AbilityReleased()
        {
            //it do not matter
        }
        public override void OnAbilityUnequipped()
        {
            //it do not matter
        }
    }
}