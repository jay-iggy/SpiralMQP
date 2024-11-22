using System;
using Game.Scripts.Interfaces;
using UnityEngine;

namespace Game.Scripts {
    public class EnemyTouchHitbox : Hitbox {
        public float dmg = 1;

        private void Awake() {
            onHitTarget.AddListener(OnHitTarget);
        }

        private void OnHitTarget(ICanGetHit target) {
            target.GetHit(dmg);
        }
    }
}