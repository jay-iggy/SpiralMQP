using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    public class VaccineAbility : MonoBehaviour
    {
        private HealthComponent playerHP;

        void Start()
        {
            playerHP = transform.parent.parent.GetComponent<HealthComponent>();
            playerHP.canHeal = false;
            CombatManager.instance.onBossDefeated.AddListener(HealRandom);
        }

        private void HealRandom()
        {
            Debug.Log("vaccine healing");
            int healing = Random.Range(1, 3);
            playerHP.Heal((float)healing, true);
        }
    }
}
