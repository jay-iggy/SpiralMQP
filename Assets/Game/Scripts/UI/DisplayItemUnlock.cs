using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemUnlock : MonoBehaviour
{
    [SerializeField] NewItemUnlocked unlockUI;
    private List<ItemPickup> itemsToDisplay = new List<ItemPickup>();


    public void UnlockItems(List<ItemPickup> items)
    {
        itemsToDisplay.AddRange(items);
        DisplayNextUnlock();
    }

    public void DisplayNextUnlock()
    {
        if (itemsToDisplay.Count == 0) return;

        NewItemUnlocked currentUnlock = Instantiate(unlockUI, transform);
        currentUnlock.onDoneDisplaying.AddListener(DisplayNextUnlock);
        ItemPickup currentItem = itemsToDisplay[0];
        itemsToDisplay.RemoveAt(0);
        currentUnlock.SetItem(currentItem);
    }
}
