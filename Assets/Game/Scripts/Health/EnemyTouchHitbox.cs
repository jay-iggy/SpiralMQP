using System;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts {
    public class EnemyTouchHitbox : Hitbox {
        public float dmg = 1;
        public UnityEvent onHit = new UnityEvent();

        private void Awake() {
            onHitTarget.AddListener(OnHitTarget);
        }

        private void OnHitTarget(ICanGetHit target) {
            target.GetHit(dmg);
            onHit.Invoke();
        }
    }
}