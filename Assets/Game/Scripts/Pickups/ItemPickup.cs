using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ItemPickup : MonoBehaviour {
    private void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TagManager.Player)) {
            ApplyEffect(other.gameObject.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect(PlayerController player);
}
