using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class shotgunAttack : MonoBehaviour, IHaveOneAttack
    {
        private GameObject bullet;
        public void SetBullet(GameObject b) { bullet = b; }

        [SerializeField] private Transform projSpawnPos;


        private Timer timer;
        private TurnHandler turner;
        private GameObject player;


        void Start()
        {
            timer = GetComponent<Timer>();
            timer.onTimerEnd.AddListener(OnTimerEnd);
            turner = projSpawnPos.GetComponent<TurnHandler>();
            player = GameObject.FindGameObjectWithTag(TagManager.Player);
        }

        public float Attack()
        {
            turner.TurnTowardsInstant(player.transform.position);
            timer.Set(.5f, 0);
            return 1;
        }

        private void ShootVolley()
        {
            Quaternion turnOffset = Quaternion.identity;
            turnOffset.eulerAngles = new Vector3(0, 90, 0);
            GameObject proj = Instantiate(bullet, projSpawnPos.position, projSpawnPos.rotation*turnOffset);
        }

        public void StopAttacking()
        {

        }

        public void OnTimerEnd(int data)
        {
            ShootVolley();
        }
    }
}
