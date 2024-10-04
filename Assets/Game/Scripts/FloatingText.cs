using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FloatingText : MonoBehaviour
{
    public float destroyDelay = 3f;

    void Start() {
        Destroy(gameObject,destroyDelay);
    }
}
