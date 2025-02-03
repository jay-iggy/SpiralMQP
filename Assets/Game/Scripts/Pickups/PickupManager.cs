using Game.Scripts.Pickups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] HealthPickup healthPickup;
        [SerializeField] ItemPickup itemPickupTemplate;
        [SerializeField] ItemPickup testItem;
        public List<ItemPickup> listOfAllItems;
        public List<ItemPickup> permanentItemPool;
        public List<ItemPickup> pickups;
        Vector3[] itemDropLocations = new Vector3[3];

        public static PickupManager instance;
        public UnityEvent onItemCollected;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                transform.parent = null;
                DontDestroyOnLoad(this);
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
            string loadedItemPool = PlayerPrefs.GetString("itemPool", "");
            DeserializeItemList(loadedItemPool);
            pickups = permanentItemPool.ToList();

            SceneManager.activeSceneChanged += populateItemPool;
        }

        public void populateItemPool(Scene current, Scene next)
        {
            pickups = permanentItemPool.ToList();
        }

        public void ReleaseItems(List<ItemPickup> items)
        {
            pickups.AddRange(items);
            permanentItemPool.AddRange(items);
            string allItems = SerializeItemList();
            PlayerPrefs.SetString("itemPool", allItems);
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
            ItemPickup item1;
            if(testItem != null)
            {
                item1 = Instantiate(testItem, itemDropLocations[1], Quaternion.identity);
            }
            else
            {
                item1 = MakeValidItem(ItemType.NONE, rarity, itemDropLocations[1]);
            }         
            if (item1 == null) return;
            ItemPickup item2 = MakeValidItem(item1.itemType, rarity, itemDropLocations[2]);
        }

        private ItemPickup MakeValidItem(ItemType excludeType, ItemRarity minRarity, Vector3 location)
        {
            if (pickups.Count == 0) return null;

            int startingIndex = Random.Range(0, pickups.Count);
            int i = startingIndex;
            ItemPickup p = null;
            ItemPickup skippedRarerItem = null;
            while(p == null)
            {
                if (pickups[i].itemType != excludeType && pickups[i].itemRarity >= minRarity)
                {
                    if(pickups[i].itemRarity == minRarity)
                    {
                        p = pickups[i];
                    }
                    else
                    {
                        int pickupChance = 0;
                        switch (pickups[i].itemRarity)
                        {
                            case ItemRarity.UNCOMMON:
                                pickupChance = 2; //one in two chance
                                break;
                            case ItemRarity.RARE:
                                pickupChance = 4;
                                break;
                        }

                        int doSpawn = Random.Range(0, pickupChance);
                        if (doSpawn == 0)
                        {
                            p = pickups[i];
                        }
                        else
                        {
                            skippedRarerItem = pickups[i];
                        }
                    }
                }
                if(p == null)
                {
                    i++;
                    if (i >= pickups.Count) i = 0;
                    if (i == startingIndex) return skippedRarerItem;
                }
            }
            ItemPickup item = Instantiate(p, location, Quaternion.identity);
            item.SetIndex(i);
            return p;
        }

        public string SerializeItemList()
        {
            string output = "";
            foreach (ItemPickup i in permanentItemPool)
            {
                int index = listOfAllItems.IndexOf(i);
                output += index;
                output +=";";
            }
            return output;
        }

        public void DeserializeItemList(string input)
        {
            if (input == "") return;

            Debug.Log("deserializing: " + input);
            string[] inputs = input.Split(';');
            List<ItemPickup> tempItemList = new List<ItemPickup>();
            foreach (string i in inputs)
            {
                if (i != "")
                {
                    int index = int.Parse(i);
                    tempItemList.Add(listOfAllItems[index]);
                }
            }
            if (tempItemList.Count > 0)
            {
                permanentItemPool = tempItemList.ToList();
            }
        }
    }

    

}
