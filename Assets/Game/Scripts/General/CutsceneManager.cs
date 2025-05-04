using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance;
        public bool playCutscene = true;
        public bool alwaysPlayCutscene = false;
        private bool cutsceneStarted = false;
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

        private void Update()
        {
            if (cutsceneStarted && Input.anyKeyDown)
            {
                EndCutscene();
            }
        }

        public void StartCutscene()
        {
            playCutscene = false;
            cutsceneStarted = true;
            timer.Set(18, 0);
        }

        public void TimeUp(int data)
        {
            if (cutsceneStarted)
            {
                EndCutscene();
            }
        }

        private void EndCutscene()
        {
            cutsceneStarted = false;
            timer.StopTimer();
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            playCutscene = alwaysPlayCutscene;
        }
        
    }
}
