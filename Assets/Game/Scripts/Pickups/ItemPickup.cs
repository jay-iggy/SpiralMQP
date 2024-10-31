using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;


public enum ItemType
{
    NONE,
    HEALTH,
    RANGED,
    MELEE,
    MOVEMENT
}

public enum ItemRarity
{
    COMMON,
    RARE,
    EPIC,
    LEGENDARY
}

[RequireComponent(typeof(Collider))]
public abstract class ItemPickup : MonoBehaviour {
    public ItemType itemType;
    public ItemRarity itemRarity;
    private int itemIndex = -1; //-1 for health pickup, ability pickups >= 0

    private void Start()
    {
        PickupManager.instance.onItemCollected.AddListener(NotSelected);
    }

    public void SetIndex(int i)
    {
        itemIndex = i;
    }

    public void NotSelected()
    {
        Destroy(gameObject);
    }

    private void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TagManager.Player)) {
            ApplyEffect(other.gameObject.GetComponent<PlayerController>());
            PickupManager.instance.ItemCollected(itemIndex);
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect(PlayerController player);
}
