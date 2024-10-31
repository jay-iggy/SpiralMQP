using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtons : MonoBehaviour
{
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnEnable() {
        Cursor.visible = true;
    }
}
