using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class MachineSmoke : MonoBehaviour
    {
        private Vector3 targetPos;
        private Vector3 targetScale;
        [SerializeField] Timer timer;

        private void Start()
        {
            timer.onTimerEnd.AddListener(OnTimerEnd);
            timer.Set(6, 0);
        }

        public void GoTo(Vector3 pos, Vector3 scale)
        {
            targetPos = pos;
            targetScale = scale;
        }

        private void FixedUpdate()
        {
            if (targetPos != null && targetScale != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, .03f);
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, .09f);
            }
        }

        public void OnTimerEnd(int i)
        {
            Destroy(gameObject);
        }
    }
}
