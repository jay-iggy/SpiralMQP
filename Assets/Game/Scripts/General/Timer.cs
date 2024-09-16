using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* how to use from another script
 * 
 * have a reference to an instance of Timer (ex. t)
 * create a method that accepts only an int (ex. onTimerEnd(int data))
 * call set, with what attack is being preformed as the data
 * subscribe the method to the timer (ex. t.timeUp.AddListener(onTimerEnd))
 */

namespace Game.Scripts
{

    public class Timer : MonoBehaviour
    {
        private float timer = -1;
        private int timerData = -1;
        public UnityEvent<int> timeUp;

        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timeUp.Invoke(timerData);
                }
            }
        }

        public void set(float time, int data)
        {
            timer = time;
            timerData = data;
        }
    }
}
