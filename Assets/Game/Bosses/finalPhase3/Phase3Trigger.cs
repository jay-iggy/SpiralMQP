using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Phase3Trigger : MonoBehaviour {
    [SerializeField] private EnemyData wizard;
    
    private void Reset() {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TagManager.Player)) {
            CombatManager.instance.nextEnemyData = wizard;
            CombatManager.instance.TransitionToNextBoss();
            Destroy(gameObject);
        }
    }
}
