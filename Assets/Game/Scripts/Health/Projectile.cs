using System;
using System.Collections.Generic;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class Projectile : Hitbox {
        public float dmg = 1;
        [SerializeField] protected bool persistent = false;
        public float speed = 0;
        public int projID = -1;
        [SerializeField] protected UnityEvent onDestroyed;
        public bool destroyedByWall = false; 
        
        protected static List<int> hitIDs = new();
        
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
            float projDmg = dmg;
            
            bool isCrit = false;
            
            // Combo: Every 3 consecutive hits, the damage is doubled
            if(projID != -1) {
                hitIDs.Add(projID);
                if(IsCombo()) {
                    projDmg *= 2;
                    hitIDs.Clear();
                    isCrit = true;
                }
            }
            
            target.GetHit(projDmg, ignoresInvincibility, isCrit);
            if(!persistent) DestroySelf();
        }

        protected override void OnTriggerEnterNonHurtbox(Collider other) {
            if (destroyedByWall && other.gameObject.CompareTag(TagManager.Wall)) {
                DestroySelf();
            }
            
        }

        public void DestroySelf()
        {
            onDestroyed.Invoke();
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

        protected bool IsCombo() {
            // Check if the last 3 hits are consecutive
            if(hitIDs.Count < 3) return false;
            for(int i = hitIDs.Count-2; i >= hitIDs.Count - 3; i--) { // check the last 3 hits
                if(hitIDs[i+1] != hitIDs[i] + 1) return false;
            }
            return true;
        }
    }
}