using System;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class Projectile : Hitbox {
        public float dmg = 1;
        
        private void Start() {
            onHitTarget.AddListener(OnHitTarget);
        }

        private void OnHitTarget(ICanGetHit target) {
            target.GetHit(dmg);
            Destroy(gameObject);
        }

        public void TargetPlayer(float speed) {
            GameObject player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            Vector3 v = Vector3.MoveTowards(transform.position, player.transform.position, speed);
            v -= transform.position;
            GetComponent<Rigidbody>().velocity = v; // expensive, we can cache the rigidbody
        }
    }
}