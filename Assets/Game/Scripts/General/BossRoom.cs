using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public static Vector3 GetRandomPositionOnWall()
    {
        Bounds bounds = instance.roomBounds.bounds;

        switch (Random.Range(1, 5)){

            case 1:
                Vector3 randomPosition = new Vector3(
          bounds.min.x,
          0,
          Random.Range(bounds.min.z, bounds.max.z));
            return randomPosition;
            case 2:
                Vector3 randomPosition1 = new Vector3(
          bounds.max.x,
          0,
          Random.Range(bounds.min.z, bounds.max.z));
                return randomPosition1;
            case 3:
                Vector3 randomPosition2 = new Vector3(
          Random.Range(bounds.min.x, bounds.max.x),
          0,
           bounds.min.z);
                return randomPosition2;
            case 4:
                Vector3 randomPosition3 = new Vector3(
          Random.Range(bounds.min.x, bounds.max.x),
          0,
           bounds.max.z);
                return randomPosition3;
        }
        return default;
    }
    public static Vector3 GetRandomPositionOnWall(int wall)
    {
        Bounds bounds = instance.roomBounds.bounds;

        switch (wall)
        {

            case 1:
                Vector3 randomPosition = new Vector3(
          bounds.min.x,
          0,
          Random.Range(bounds.min.z, bounds.max.z));
                return randomPosition;
            case 2:
                Vector3 randomPosition1 = new Vector3(
          bounds.max.x,
          0,
          Random.Range(bounds.min.z, bounds.max.z));
                return randomPosition1;
            case 3:
                Vector3 randomPosition2 = new Vector3(
          Random.Range(bounds.min.x, bounds.max.x),
          0,
           bounds.min.z);
                return randomPosition2;
            case 4:
                Vector3 randomPosition3 = new Vector3(
          Random.Range(bounds.min.x, bounds.max.x),
          0,
           bounds.max.z);
                return randomPosition3;
        }
        return default;
    }

    public static Vector3 GetRandomPositionOnWallcorner(int wall)
    {
        Bounds bounds = instance.roomBounds.bounds;
        float a = ((bounds.max.x - bounds.min.x) / 2) + bounds.min.x;
        float b = ((bounds.max.z - bounds.min.z) / 2) + bounds.min.z;
        switch (wall)
        {

            case 1:

                if (Random.Range(0, 2) == 0)
                {
                    Vector3 randomPosition = new Vector3(
          bounds.min.x,
          0,
          Random.Range(bounds.min.z, b));
                    return randomPosition;
                }
                else {
                    Vector3 randomPosition = new Vector3(
          Random.Range(bounds.min.x, a),
          0,
          bounds.min.z);
                    return randomPosition;
                }

                
            case 2:

                if (Random.Range(0, 2) == 0)
                {
                    Vector3 randomPosition = new Vector3(
          bounds.max.x,
          0,
          Random.Range(bounds.min.z, b));
                    return randomPosition;
                }
                else
                {
                    Vector3 randomPosition = new Vector3(
          Random.Range(a,bounds.max.x),
          0,
          bounds.min.z);
                    return randomPosition;
                }


            case 3:

                if (Random.Range(0, 2) == 0)
                {
                    Vector3 randomPosition = new Vector3(
          bounds.min.x,
          0,
          Random.Range(b,bounds.max.z));
                    return randomPosition;
                }
                else
                {
                    Vector3 randomPosition = new Vector3(
          Random.Range(bounds.min.x, a),
          0,
          bounds.max.z);
                    return randomPosition;
                }

            case 4:

                if (Random.Range(0, 2) == 0)
                {
                    Vector3 randomPosition = new Vector3(
          bounds.max.x,
          0,
          Random.Range(b,bounds.max.z));
                    return randomPosition;
                }
                else
                {
                    Vector3 randomPosition = new Vector3(
          Random.Range(a,bounds.max.x),
          0,
          bounds.max.z);
                    return randomPosition;
                }

        }
        return default;
    }
}
