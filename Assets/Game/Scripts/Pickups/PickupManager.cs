using Game.Scripts.Pickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] HealthPickup healthPickup;
        [SerializeField] List<ItemPickup> pickups;
        Vector3[] itemDropLocations = new Vector3[3];

        public static PickupManager instance;
        public UnityEvent onItemCollected;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            for(int i = 0; i<itemDropLocations.Length; i++)
            {
                itemDropLocations[i] = transform.GetChild(i).position;
            }
        }

        public void ItemCollected(int index)
        {
            CombatManager.instance.TransitionToNextBoss();
            if (index != -1)
            {
                pickups.RemoveAt(index);
            }
            onItemCollected.Invoke();
        }

        public void DropItems(ItemRarity rarity)
        {
            Instantiate(healthPickup, itemDropLocations[0], Quaternion.identity);
            ItemPickup item1 = MakeValidItem(ItemType.NONE, rarity, itemDropLocations[1]);
            if (item1 == null) return;
            ItemPickup item2 = MakeValidItem(item1.itemType, rarity, itemDropLocations[2]);
        }

        private ItemPickup MakeValidItem(ItemType excludeType, ItemRarity minRarity, Vector3 location)
        {
            if (pickups.Count == 0) return null;

            int startingIndex = Random.Range(0, pickups.Count);
            int i = startingIndex;
            ItemPickup p = null;
            while(p == null)
            {
                if (pickups[i].itemType != excludeType)
                {
                    p = pickups[i];
                }
                else
                {
                    i++;
                    if (i >= pickups.Count) i = 0;
                    if (i == startingIndex) return null;
                }
            }
            ItemPickup item = Instantiate(p, location, Quaternion.identity);
            item.SetIndex(i);
            return p;
        }
    }

}
