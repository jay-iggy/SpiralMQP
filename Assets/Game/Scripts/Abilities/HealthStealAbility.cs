using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    public class HealthStealAbility : MonoBehaviour
    {
        private float damageDone = 0;
        private HealthComponent bossHP;
        private HealthComponent playerHP;
        [SerializeField] int damageThreshold;

        public Sound sfx;

        private void Start()
        {
            CombatManager.instance.onBossSpawned.AddListener(setBossHP);          
            playerHP = transform.parent.parent.GetComponent<HealthComponent>();
            playerHP.onTakeDamage.AddListener(resetDamageDone);
        }

        public void setBossHP()
        {
            bossHP = CombatManager.instance.currentBoss.GetComponent<HealthComponent>();
            bossHP.onTakeDamageFloat.AddListener(trackDamageDone);
        }

        public void trackDamageDone(float d)
        {
            damageDone += d;
            if(damageDone >= damageThreshold)
            {
                playerHP.SetHealth(playerHP.health + 1);
                damageDone = 0;
                PlaySound();
            }
        }

        public void resetDamageDone()
        {
            damageDone = 0;
        }
        private void PlaySound() {
            if(sfx != null) sfx.PlaySound();
        }
    }
}
