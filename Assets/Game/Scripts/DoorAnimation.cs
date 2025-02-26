using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        PickupManager.instance.itemSpawnLocation = transform;
    }

    private void OnEnable() {
        CombatManager.instance.onBossDefeated.AddListener(OpenDoor);
        CombatManager.instance.onTransitionToFinalBoss.AddListener(OpenDoor);
    }

    private void OnDisable() {
        CombatManager.instance.onBossDefeated.RemoveListener(OpenDoor);
        CombatManager.instance.onTransitionToFinalBoss.RemoveListener(OpenDoor);
    }

    public void OpenDoor() {
        animator.SetTrigger("open");
    }
}
