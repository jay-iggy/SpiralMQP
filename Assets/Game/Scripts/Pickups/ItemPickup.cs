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
    MOVEMENT,
    PASSIVE
}

public enum ItemRarity
{
    COMMON,
    UNCOMMON,
    RARE
}

[RequireComponent(typeof(Collider))]
public abstract class ItemPickup : MonoBehaviour {
    public string itemName;
    public ItemType itemType;
    public ItemRarity itemRarity;
    public string itemDescription;
    private int itemIndex = -1; //-1 for health pickup, ability pickups >= 0
    private float gracePeriod = .1f;
    public GameObject pickupUIPrefab;

    protected void Start()
    {
        PickupManager.instance.onItemCollected.AddListener(NotSelected);
        GameObject pickupUI = Instantiate(pickupUIPrefab, this.transform);
        pickupUI.GetComponent<PickupUI>().updateValues(itemName, itemType, itemRarity, itemDescription);
    }

    private void Update()
    {
        if(gracePeriod > 0f) { 
            gracePeriod -= Time.deltaTime; 
        }
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
        if (gracePeriod > 0) return;

        if (other.CompareTag(TagManager.Player)) {
            ApplyEffect(other.gameObject.GetComponent<PlayerController>());
            PickupManager.instance.ItemCollected(itemIndex);
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect(PlayerController player);
}
