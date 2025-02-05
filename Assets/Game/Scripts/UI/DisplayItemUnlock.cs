using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemUnlock : MonoBehaviour
{
    [SerializeField] GameObject unlockUI;
    private List<ItemPickup> itemsToDisplay = new List<ItemPickup>();

    private void Start()
    {
        Instantiate(unlockUI, transform);
    }

    public void UnlockItem(List<ItemPickup> items)
    {

    }
}
