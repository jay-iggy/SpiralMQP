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
    }
}