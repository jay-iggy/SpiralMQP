using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private bool preserveRotation = false;
    public void Spawn() {
        GameObject obj = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        if (preserveRotation) {
            obj.transform.eulerAngles = objectToSpawn.transform.eulerAngles;
        }
    }
}
