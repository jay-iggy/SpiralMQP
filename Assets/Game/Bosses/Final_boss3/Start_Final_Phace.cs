using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Start_Final_Phace : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent StartFinalFight = new();
    public void StartFinal()
    {
        StartFinalFight.Invoke();
    }
}
