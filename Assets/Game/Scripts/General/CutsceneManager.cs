using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance;
        public bool playCutscene = true;
        [SerializeField] Timer timer;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            timer.onTimerEnd.AddListener(TimeUp);
        }

        public void StartCutscene()
        {
            playCutscene = false;
            timer.Set(18, 0);
        }

        public void TimeUp(int data)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        
    }
}
