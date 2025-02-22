using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class DestroyedByProjectile : MonoBehaviour {
    [SerializeField] private string tag;

    private void OnTriggerEnter(Collider other) {
        Projectile projectile = other.GetComponent<Projectile>();
        if(projectile != null && projectile.tagsToHit.Contains(tag)) {
            Destroy(gameObject);
            Destroy(projectile.gameObject);
        }
    }
}
