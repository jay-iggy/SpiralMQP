using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSwap : MonoBehaviour {
    public GameObject arena1;
    public GameObject arena2;
    
    public Camera camera;
    
    private bool isArena1Active = true;
    
    private Vector3 cameraPos1;
    private Vector3 cameraRot1;
    private float cameraFOV1;
    
    public Vector3 cameraPos2 = new Vector3(0,32,-10.38f);
    public Vector3 cameraRot2 = new Vector3(75,0,0);
    public float cameraFOV2 = 75f;
    
    public static ArenaSwap instance;
    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    void Start() {
        cameraPos1 = camera.transform.position;
        cameraRot1 = camera.transform.eulerAngles;
        cameraFOV1 = camera.fieldOfView;
    }
    
    public void SwapArena() {
        arena1.SetActive(!arena1.activeSelf);
        arena2.SetActive(!arena2.activeSelf);
        
        
        switch(isArena1Active) {
            case true:
                camera.transform.position = cameraPos2;
                camera.fieldOfView = cameraFOV2;
                camera.transform.eulerAngles = cameraRot2;
                break;
            case false:
                camera.transform.position = cameraPos1;
                camera.fieldOfView = cameraFOV1;
                camera.transform.eulerAngles = cameraRot1;
                break;
        }
        
        isArena1Active = !isArena1Active;
        
    }
}
