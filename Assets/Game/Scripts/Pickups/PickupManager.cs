using Game.Scripts.Pickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] HealthPickup healthPickup;
        [SerializeField] List<AbilityPickup> abilityPickups;
        Vector3[] itemDropLocations = new Vector3[3];

        public static PickupManager instance;

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

        public void DropItems(ItemRarity rarity)
        {
            Instantiate(healthPickup, itemDropLocations[0], Quaternion.identity);
            AbilityPickup item1 = NextValidItem(ItemType.NONE, rarity);
            if (item1 == null) return;
            AbilityPickup item2 = NextValidItem(item1.itemType, rarity);
            Instantiate(item1, itemDropLocations[1], Quaternion.identity);
            if (item2 == null) return;
            Instantiate(item2, itemDropLocations[2], Quaternion.identity);
        }

        private AbilityPickup NextValidItem(ItemType excludeType, ItemRarity minRarity)
        {
            if (abilityPickups.Count == 0) return null;

            int i = 0;
            AbilityPickup a = null;
            while(a == null)
            {
                if (abilityPickups[i].itemType != excludeType)
                {
                    a = abilityPickups[i];
                }
                else
                {
                    i++;
                    if (i >= abilityPickups.Count) return null;
                }
            }
            abilityPickups.RemoveAt(i);
            return a;
        }
    }

}
