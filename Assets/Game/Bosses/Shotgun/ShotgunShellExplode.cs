using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class ShotgunShellExplode : MonoBehaviour {
    [SerializeField] Projectile bulletPrefab;
    [SerializeField] int bulletCount = 8;
    [SerializeField] float bulletSpeed = 8;
    
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Projectile>() != null) {
            Explode();
        }
    }

    private void Explode() {
        CreateBulletCircle();
        Destroy(gameObject);
        // TODO: cleanup projectiles created by explosion
    }

    private void CreateBulletCircle() {
        List<GameObject> bullets = new List<GameObject>();
        for (int i = 0; i < bulletCount; i++) {
            bullets.Add(Instantiate(bulletPrefab).gameObject);
            BulletPatterns.CreateCircle(bullets.ToArray(), transform.position, 1);
        }
        BulletPatterns.MoveTowards(bullets.ToArray(),transform.position, -bulletSpeed); // bullets move away from the center
    }
}
