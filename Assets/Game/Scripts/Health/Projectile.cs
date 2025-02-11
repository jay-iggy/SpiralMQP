using System;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class Projectile : Hitbox {
        public float dmg = 1;
        [SerializeField] protected bool persistent = false;
        public float speed = 0;
        
        //TODO: destroy on hit wall
        
        private void Awake() {
            onHitTarget.AddListener(OnHitTarget);
            if(speed != 0 && GetComponent<Rigidbody>() != null){
                GetComponent<Rigidbody>().velocity = transform.forward * speed;
            }
        }

        public void IgnoreInvincibility()
        {
            ignoresInvincibility = true;
        }

        virtual protected void OnHitTarget(ICanGetHit target) {
            target.GetHit(dmg, ignoresInvincibility);
            if(!persistent) DestroySelf();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void TargetPlayer(float speed) {
            GameObject player = GameObject.FindGameObjectWithTag(TagManager.Player); // expensive, we can just make the player a singleton
            if(player == null) {
                Debug.Log("Projectile.TargetPlayer(): No player found");
                return;
            }
            Vector3 v = player.transform.position-transform.position;
            v.Normalize();
            v *= speed;
            GetComponent<Rigidbody>().velocity = v; // expensive, we can cache the rigidbody
        }
    }
}