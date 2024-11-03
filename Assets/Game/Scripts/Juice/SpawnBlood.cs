using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlood : MonoBehaviour{
    public ParticleSystem bloodPrefab;
    public void Spawn() {
        ParticleSystem p = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        p.Play();
    }
}
