using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameOverButtons : MonoBehaviour
{
    public UnityEvent onMenuEnable;
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnEnable() {
        Cursor.visible = true;
        onMenuEnable.Invoke();
    }
}
