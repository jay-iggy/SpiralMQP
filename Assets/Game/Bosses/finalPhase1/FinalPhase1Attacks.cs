using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts {
    public class FinalPhase1Attacks : MonoBehaviour, ICanAttack
    {
        [SerializeField] GameObject shield;
        private GameObject shieldDamage;
        [SerializeField] GameObject[] pillarPrefabs;
        [SerializeField] Vector3[] pillarPositions;
        private GameObject[] pillars = new GameObject[6];
        private PillarController[] controllers = new PillarController[6];
        private int deadPillars = 0;

        private TurnHandler turnHandler;
        private Timer timer;
        private GameObject player;
        private Vector3 chargeTarget;
        private bool charging = false;
        private float chargeSpeed = .15f;
        private bool isAttacking = false;


        public int GetAttackCount() { return 1; }

        public float Attack(int index)
        {
            if (isAttacking) InitiateCharge();

            return 0;
        }
        void Start()
        {
            shieldDamage = shield.transform.GetChild(0).gameObject;
            turnHandler = transform.GetChild(0).GetComponent<TurnHandler>();
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
            player = GameObject.FindGameObjectWithTag(TagManager.Player);

            for (int i = 0; i<pillarPrefabs.Length; i++)
            {
                pillars[i] = Instantiate(pillarPrefabs[i], pillarPositions[i], Quaternion.identity);
                controllers[i] = pillars[i].transform.GetChild(0).GetComponent<PillarController>();
                if (controllers[i] != null)
                {
                    controllers[i].mainBoss = this;
                }
            }
        }

        private void InitiateCharge()
        {
            turnHandler.TurnTowardsInstant(player.transform.position);
            chargeTarget = player.transform.position;
            timer.Set(.5f, 0);
        }

        public void KillPillar()
        {
            deadPillars++;
            for(int i = 0;i<controllers.Length; i++)
            {
                if (controllers[i] != null)
                {
                    controllers[i].attackDelay -= .1f;
                }
            }

            if(deadPillars == 1)
            {
                isAttacking = true;
            }
            else if (deadPillars >= 6)
            {
                isAttacking = false;
                shield.SetActive(false);
                GetComponent<HealthComponent>().isInvincible = false;
            }
            else if (deadPillars >= 3)
            {
                GetComponent<Boss>().attackDelay -= .8f;
            }
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    charging = true;
                    shieldDamage.SetActive(true);
                    break;
            }
        }

        void FixedUpdate()
        {
            if (charging)
            {
                Vector3 direction = chargeTarget - transform.position;
                if (direction.magnitude < chargeSpeed)
                {
                    charging = false;
                    shieldDamage.SetActive(false);
                    return;
                }

                direction.Normalize();
                direction *= chargeSpeed;
                transform.position += direction;
            }
        }

    }
}
