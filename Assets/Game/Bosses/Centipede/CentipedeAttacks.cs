using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class CentipedeAttacks : MonoBehaviour, ICanAttack {
        private const int CHASE = 0;
        private const int GROW_TURRET = 1;

        [SerializeField] private GameObject centipedeHead;
        public float speed;

        public string target; // player or snack
        private Transform player;
        private Transform snack;
        [SerializeField] private GameObject snackPrefab;
        [SerializeField] private float snackWaitTime;

        [SerializeField] Timer timer;

        float lastHealth;
        float lastPlayerHealth;

        private int curAttack = -1;

        private void Start() {
            player = GameObject.FindGameObjectWithTag(TagManager.Player).transform; // expensive, we can just make the player a singleton
            timer.onTimerEnd.AddListener(OnTimerEnd);

            lastHealth = GetComponent<HealthComponent>().health;
            lastPlayerHealth = player.GetComponent<HealthComponent>().health;
        }

        public int GetAttackCount() { return 1; }

        public float Attack(int index) {
            curAttack = index;

            switch (curAttack) {
                case CHASE:
                    return Chase();
            }

            return 0;
        }

        private float Chase()
        {
            if(snack != null)
            {
                Vector3 directionToTarget1 = player.transform.position - centipedeHead.transform.position;
                Vector3 directionToTarget2 = snack.transform.position - centipedeHead.transform.position;
                directionToTarget1.y = 0;
                directionToTarget2.y = 0;
                // Calculate the angle between the forward direction of each object and the direction to the target
                float rotForPlayer = Vector3.Angle(centipedeHead.transform.forward, directionToTarget1);
                float rotForSnack = Vector3.Angle(centipedeHead.transform.forward, directionToTarget2);

                // Check which object requires less rotation to face the target
                if (rotForPlayer < rotForSnack)
                {
                    // head to player
                    centipedeHead.transform.rotation = Quaternion.RotateTowards(centipedeHead.transform.rotation, Quaternion.LookRotation(directionToTarget1), speed * Time.deltaTime);
                    centipedeHead.transform.position = Vector3.MoveTowards(centipedeHead.transform.position, player.position, speed);
                    target = "player";
                }
                else if (rotForSnack <= rotForPlayer)
                {
                    // head to snack
                    centipedeHead.transform.rotation = Quaternion.RotateTowards(centipedeHead.transform.rotation, Quaternion.LookRotation(directionToTarget2), speed * Time.deltaTime);
                    centipedeHead.transform.position = Vector3.MoveTowards(centipedeHead.transform.position, snack.position, speed);
                    target = "snack";
                }
            }
            else
            {
                // head to player
                Vector3 directionToTarget1 = player.transform.position - centipedeHead.transform.position;
                centipedeHead.transform.rotation = Quaternion.RotateTowards(centipedeHead.transform.rotation, Quaternion.LookRotation(directionToTarget1), speed * Time.deltaTime);
                centipedeHead.transform.position = Vector3.MoveTowards(centipedeHead.transform.position, player.position, speed);
                target = "player";
            }

            return 1;
        }

        public void OnTimerEnd(int data) {
            switch (data) {
                case CHASE:
                    break;
                case GROW_TURRET:
                    break;
            }
        }

        private void FixedUpdate() {
            switch (curAttack) {
                case CHASE:
                    if(target == "player")
                    {
                        centipedeHead.transform.rotation = Quaternion.RotateTowards(centipedeHead.transform.rotation, Quaternion.LookRotation(player.transform.position - centipedeHead.transform.position), speed * 10000 * Time.deltaTime);
                        centipedeHead.transform.position = Vector3.MoveTowards(centipedeHead.transform.position, player.position, speed * 2);
                    }
                    else if (target == "snack" && snack != null)
                    {
                        centipedeHead.transform.rotation = Quaternion.RotateTowards(centipedeHead.transform.rotation, Quaternion.LookRotation(snack.transform.position - centipedeHead.transform.position), speed * 10000 * Time.deltaTime);
                        centipedeHead.transform.position = Vector3.MoveTowards(centipedeHead.transform.position, snack.position, speed);
                    }
                    break;
                case GROW_TURRET:
                    break;
            }

            // always 1 snack on stage
            if(snack == null)
            {
                Vector3 randLoc = new Vector3(Random.Range(-9,9), 1, Random.Range(-4,4));
                while (Vector3.Distance(randLoc, centipedeHead.transform.position) < 1.5)
                    randLoc = new Vector3(Random.Range(-9, 9), 1, Random.Range(-4, 4));
                snack = Instantiate(snackPrefab, randLoc, Quaternion.identity).transform;
            }

            HealthComponent healthComponent = GetComponent<HealthComponent>();
            lastHealth = healthComponent.health;

            if (Input.GetKeyDown("-"))
            {
                healthComponent.TakeDamage(100);
            }
            if(healthComponent.health < lastHealth)
            {
                // taken damage
                GetComponentInChildren<CentipedeChain>().RemoveLastSegment();
            }

            if(player.GetComponent<HealthComponent>().health < lastPlayerHealth)
            {
                // health changed
                target = "snack";
                lastPlayerHealth = player.GetComponent<HealthComponent>().health;
            }
        }
    }
}
