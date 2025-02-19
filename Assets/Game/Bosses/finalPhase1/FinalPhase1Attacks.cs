using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts {
    public class FinalPhase1Attacks : MonoBehaviour, ICanAttack
    {
        [SerializeField] GameObject shield;
        [SerializeField] GameObject[] pillarPrefabs;
        [SerializeField] Vector3[] pillarPositions;
        private GameObject[] pillars = new GameObject[6];
        private int deadPillars = 0;

        private int numAttacks = 0;

        public int GetAttackCount() { return numAttacks; }

        public float Attack(int index)
        {
            return 0;
        }
        void Start()
        {
            for(int i = 0; i<pillarPrefabs.Length; i++)
            {
                pillars[i] = Instantiate(pillarPrefabs[i], pillarPositions[i], Quaternion.identity);
                PillarController pc = pillars[i].transform.GetChild(0).GetComponent<PillarController>();
                if (pc != null)
                {
                    pc.mainBoss = this;
                }
            }
        }

        public void KillPillar()
        {
            deadPillars++;
            if (deadPillars >= 6)
            {
                shield.SetActive(false);
                GetComponent<HealthComponent>().invincible = false;
            }
        }

    }
}
