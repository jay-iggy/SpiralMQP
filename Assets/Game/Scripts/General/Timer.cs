using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/* how to use from another script
 * 
 * have a reference to an instance of Timer (ex. t)
 * create a method that accepts only an int (ex. onTimerEnd(int data))
 * int Start(), subscribe the method to the timeUp event (ex. t.timeUp.AddListener(onTimerEnd))
 */

namespace Game.Scripts
{

    public class Timer : MonoBehaviour
    {
        private float timer = -1;
        private int timerData = -1;
        public UnityEvent<int> onTimerEnd;

        void Update() {
            if (timer > 0) {
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    onTimerEnd.Invoke(timerData);
                }
            }
        }

        public void Set(float time, int data) {
            timer = time;
            timerData = data;
        }
    }
}
