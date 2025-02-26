using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FindUICamera : MonoBehaviour {
    private void Start() {
        GetComponent<Canvas>().worldCamera = UICamera.instance.camera;
    }
}
