using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BossRoom : MonoBehaviour {
    public static BossRoom instance;
    
    [HideInInspector]public BoxCollider roomBounds;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        
        roomBounds = GetComponent<BoxCollider>();
    }
    
    public static Vector3 GetRandomPositionInRoom() {
        Bounds bounds = instance.roomBounds.bounds;
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
        return randomPosition;
    }
    
    public static Vector3 GetRandomPositionInRoom(float padding) {
        Bounds bounds = instance.roomBounds.bounds;
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x + padding, bounds.max.x - padding),
            0,
            Random.Range(bounds.min.z + padding, bounds.max.z - padding)
        );
        return randomPosition;
    }
    
}
