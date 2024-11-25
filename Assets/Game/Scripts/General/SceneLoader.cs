using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class SceneLoader : MonoBehaviour
    {

        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void Play()
        {
            if (CutsceneManager.instance.playCutscene)
            {
                CutsceneManager.instance.StartCutscene();
                LoadScene("IntroCutscene");
            }
            else
            {
                LoadScene("SampleScene");
            }
        }

        public void NewPlayer()
        {
            CutsceneManager.instance.playCutscene = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
