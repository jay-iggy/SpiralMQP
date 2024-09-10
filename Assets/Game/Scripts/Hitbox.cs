using System.Collections.Generic;
using Game.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Scripts {
    public class Hitbox : MonoBehaviour {
        
        // how to make this manage several separate colliders?
        // challenges: the list of tags and the event can exists on one hub, but ontriggerenter of all colliders needs to link back to that hub
        
        [SerializeField] private List<string> tagsToHit = new List<string>();
        
        public UnityEvent<IHurtbox> onHit; // the parent of the hitbox should subscribe to this event to handle the hit (call the hit method on the hurtbox)
        
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out IHurtbox hurtbox)) {
                string hurtboxName = other.gameObject.tag;
                if(tagsToHit.Contains(hurtboxName)) {
                    onHit.Invoke(hurtbox);
                }
            }
        }
    }
}