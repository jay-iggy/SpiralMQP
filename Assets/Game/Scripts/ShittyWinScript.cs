﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts {
    public class ShittyWinScript : MonoBehaviour {
        public float delay = 1.5f;

        public UnityEvent onWin;
        
        public void Win() {
            StartCoroutine(Delay());
        }

        private IEnumerator Delay() {
            yield return new WaitForSeconds(delay);
            onWin.Invoke();
        }


        public void Quit() {
            StartCoroutine(QuitAfterDelay());
        }
        public IEnumerator QuitAfterDelay() {
            yield return new WaitForSeconds(delay);
            Application.Quit();
        }
    }
}