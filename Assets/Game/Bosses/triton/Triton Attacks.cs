using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class TritonAttacks : MonoBehaviour, ICanAttack
    {
        public int GetAttackCount() { return 3; }
        [SerializeField] Shark sharkPrefab;
        private Timer timer;
        private Shark shark;

        void Start()
        {
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
            shark = Instantiate(sharkPrefab);
        }
        
        public float Attack(int index)
        {
            switch (index)
            {
                case 0:
                    return SummonShark();
                case 1:
                    return SummonWave();
                case 2:
                    return ThrowTrident();
                case 3:
                    return ChasePlayer();
            }

            return 0;
        }

        public float SummonShark()
        {
            if (shark.state != SharkState.SUBMERGED) return 1;

            Debug.Log("summoning shark");
            shark.turnHandler.turnDelta = 4;
            timer.Set(.5f, 0);
            shark.state = SharkState.CHASING;
            return 2;
        }

        public float SummonWave()
        {
            return 2;
        }

        public float ThrowTrident()
        {
            return 1;
        }

        public float ChasePlayer()
        {
            return 5;
        }

        public void OnTimerEnd(int data)
        {
            switch (data)
            {
                case 0:
                    shark.turnHandler.turnDelta = 1;
                    break;
            }
        }
    }
}
