using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class TritonAttacks : MonoBehaviour, ICanAttack
    {
        const int RESET_TRIDENT = 5;
        private GameObject player;
        private Vector3 center = new Vector3(0, 2, 0);
        [SerializeField] Shark sharkPrefab;
        [SerializeField] GameObject waterPrefab;
        [SerializeField] GameObject[] tridents; //prefab, default, up, left, right, thrown
        private float[] waterPos = { -33.2f, -15.2f, 33, 15}; //left out, left in, right out, right in
        private Timer timer;
        private Shark shark;
        private GameObject water;
        private float waterVel = 0;
        private float waterTarget = 0;
        private int curAttack = -2;
        private float speed;


        public int GetAttackCount() { return 3; }

        void Start()
        {
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
            shark = Instantiate(sharkPrefab);
        }
        
        public float Attack(int index)
        {
            curAttack = index;
            switch (index)
            {
                case 0:
                    return SummonShark();
                case 1:
                    return SummonWave();
                case 2:
                    return GoToCenter();
            }

            return 0;
        }

        private float RunAway()
        {
            curAttack = -1;
            return 1;
        }

        public float SummonShark()
        {
            if (shark.state != SharkState.SUBMERGED) return RunAway();

            tridents[1].SetActive(false);
            tridents[2].SetActive(true);

            shark.turnHandler.turnDelta = 4;
            timer.Set(.5f, 0);
            shark.state = SharkState.CHASING;
            return 2;
        }

        public void KillShark()
        {
            Destroy(shark.gameObject);
        }

        private float SummonWave()
        {
            if (water != null) return RunAway();

            tridents[1].SetActive(false);
            timer.Set(2, RESET_TRIDENT);

            water = Instantiate(waterPrefab);
            if(Random.Range(0, 2) == 0)
            {
                tridents[4].SetActive(true);
                water.transform.position = new Vector3(waterPos[0] ,water.transform.position.y, water.transform.position.z);
                waterTarget = waterPos[1];
                waterVel = .1f;
            }
            else
            {
                tridents[3].SetActive(true);
                water.transform.position = new Vector3(waterPos[2], water.transform.position.y, water.transform.position.z);
                waterTarget = waterPos[3];
                waterVel = -.1f;
            }
            return 2;
        }

        private float GoToCenter()
        {
            speed = Vector3.Distance(center, transform.position) / 50;
            timer.Set(1.4f, 2);
            return 2.5f;
        }

        private void ThrowTrident()
        {
            tridents[1].SetActive(false);
            tridents[5] = Instantiate(tridents[0]);
            tridents[5].transform.position = tridents[1].transform.position;
            tridents[5].GetComponent<TurnHandler>().TurnTowardsInstant(player.transform.position);
            tridents[5].GetComponent<Projectile>().TargetPlayer(12);
            timer.Set(1, RESET_TRIDENT);
        }

        void FixedUpdate()
        {
            if (water != null)
            {
                water.transform.position += Vector3.right * waterVel;
                if (waterTarget > 0 && water.transform.position.x < waterTarget)
                {
                    waterVel += .005f;
                }
                else if (waterTarget < 0 && water.transform.position.x > waterTarget)
                {
                    waterVel -= .005f;
                }

                if (water.transform.position.x < waterPos[0] || water.transform.position.x > waterPos[2])
                {
                    Destroy(water);
                }
            }

            if (curAttack == -1)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -.03f);
            }
            else if(curAttack == 2)
            {
                transform.position = Vector3.MoveTowards(transform.position, center, speed);
            }
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    shark.turnHandler.turnDelta = 1;
                    timer.Set(1.5f, RESET_TRIDENT);
                    break;
                case 2:
                    curAttack = -2;
                    ThrowTrident();
                    break;
                case RESET_TRIDENT:
                    if (tridents[5] != null) Destroy(tridents[5]);

                    for(int i = 2; i<5; i++)
                    {
                        tridents[i].SetActive(false);
                    }
                    tridents[1].SetActive(true);
                    break;
            }
        }
    }
}
