using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour {
    public static UICamera instance;
    [HideInInspector]public Camera camera;
    
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        camera = GetComponent<Camera>();
    }
}
