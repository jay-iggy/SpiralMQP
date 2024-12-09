using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pausemenu : MonoBehaviour
{
    private bool ispaued;
    private bool key_relaces;
    public Canvas pausemenu;
    public UnityEvent onpause; 

// Start is called before the first frame update
void Start()
    {
        ispaued = false;
        key_relaces =true;
        pausemenu.GetComponent<Canvas>().enabled = false;
    }

// Update is called once per frame
void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !ispaued && key_relaces)
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.P) && ispaued && key_relaces)
        {
            unPause();
        }
        if (!Input.GetKeyDown(KeyCode.P))
        {
            key_relaces = true;
        }
    }
    public void Pause()
    {
        Time.timeScale = 0;
        ispaued = true;
        key_relaces = false;
        onpause.Invoke();
        pausemenu.GetComponent<Canvas>().enabled = true;

    }
    public void unPause()
    {
        Time.timeScale = 1;
        ispaued = false;
        key_relaces = false;
        pausemenu.GetComponent<Canvas>().enabled = false;
    }
}
