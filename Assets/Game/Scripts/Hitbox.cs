using System.Collections.Generic;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class Hitbox : MonoBehaviour {
        
        // how to make this manage several separate colliders?
        // the list of tags and the event can exists on one hub, but ontriggerenter of all colliders needs to link back to that hub
        // comment: a hub could subscribe its function to all events of hitboxes
        
        [SerializeField] private List<string> tagsToHit = new List<string>();
        
        public UnityEvent<ICanGetHit> onHitTarget; // the parent of the hitbox should subscribe to this event to handle the hit (call the hit method on the hurtbox)
        
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out ICanGetHit hurtbox)) {
                string hurtboxName = other.gameObject.tag;
                if(tagsToHit.Contains(hurtboxName)) {
                    onHitTarget.Invoke(hurtbox);
                }
            }
        }
    }
}