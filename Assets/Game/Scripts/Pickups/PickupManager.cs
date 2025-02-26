using System;
using Game.Scripts.Pickups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Scripts {
    public class PickupManager : MonoBehaviour {
        [SerializeField] bool deleteSave = false;
        [SerializeField] DisplayItemUnlock canvas;
        [SerializeField] HealthPickup healthPickup;
        [SerializeField] ItemPickup testItem;
        public List<ItemPickup> listOfAllItems;
        public List<ItemPickup> permanentItemPool;
        public List<ItemPickup> pickups;
        public ItemPickup the20DollarBill;
        Vector3[] itemDropLocations = new Vector3[3];

        public static PickupManager instance;
        public UnityEvent onItemCollected;

        [SerializeField] private AnimationCurve throwHeightCurve;
        public Transform itemSpawnLocation;
        [SerializeField] float itemThrowDuration = 1f;
        
        private void Awake() {
            if (instance == null) {
                instance = this;
                /*transform.parent = null;
                DontDestroyOnLoad(this);*/
            }
            else {
                Destroy(gameObject);
            }
        }
        
        private void OnEnable() {
            CombatManager.instance.onBossDefeated.AddListener(OnBossDefeated);
            CombatManager.instance.onTransitionToFinalBoss.AddListener(OnFinalBossTransition);
        }
        private void OnDisable() {
            CombatManager.instance.onBossDefeated.RemoveListener(OnBossDefeated);
            CombatManager.instance.onTransitionToFinalBoss.RemoveListener(OnFinalBossTransition);
        }

        void Start() {
            if (deleteSave) PlayerPrefs.DeleteAll();

            for(int i = 0; i<itemDropLocations.Length; i++) { // create spawn locations
                itemDropLocations[i] = transform.GetChild(i).position;
            }
            string loadedItemPool = PlayerPrefs.GetString("itemPool", "");
            DeserializeItemList(loadedItemPool);
            pickups = permanentItemPool.ToList();

            SceneManager.activeSceneChanged += PopulateItemPool;
            
            // disable test item in build
            #if UNITY_EDITOR
            #else
                testItem=null;
            #endif
        }

        public void PopulateItemPool(Scene current, Scene next) {
            pickups = permanentItemPool.ToList();
        }

        public void UnlockItems(List<ItemPickup> items) {
            if(items.Count == 0) return;

            if(canvas!=null) {
                canvas.UnlockItems(items);
            }
            foreach (ItemPickup item in items) {
                if (!permanentItemPool.Contains(item)) {
                    permanentItemPool.Add(item);
                    pickups.Add(item);
                }
            }
            
            string allItems = SerializeItemList();
            PlayerPrefs.SetString("itemPool", allItems);
        }
        
        private void OnBossDefeated() {
            EnemyData defeatedEnemy = CombatManager.instance.currentEnemyData;

            string bossKey = "boss" + defeatedEnemy.bossIndex + "defeated";
            bool hasBeenDefeated = PlayerPrefs.GetInt(bossKey, 0) == 1;
            
            if (!hasBeenDefeated && defeatedEnemy.unlockedItems.Count>0) {
                PickupManager.instance.UnlockItems(defeatedEnemy.unlockedItems);
            }
            
            PickupManager.instance.DropItems(defeatedEnemy.minItemRarity);
        }
        private void OnFinalBossTransition() {
            if (the20DollarBill != null) {
                SpawnItem(the20DollarBill, itemDropLocations[1]);
            }
        }

        public void DropItems(ItemRarity rarity) {
            //Instantiate(healthPickup, itemDropLocations[0], Quaternion.identity);
            SpawnItem(healthPickup, itemDropLocations[0]);
            ItemPickup item1;
            if(testItem != null) {
                item1 = SpawnItem(testItem, itemDropLocations[1]);
            }
            else {
                item1 = MakeValidItem(ItemType.NONE, rarity, itemDropLocations[1]);
            }         
            if (item1 == null) return;
            ItemPickup item2 = MakeValidItem(item1.itemType, rarity, itemDropLocations[2]);
        }
        public void OnItemCollected(int index) {
            CombatManager.instance.TransitionToNextBoss();
            if (index != -1) {
                pickups.RemoveAt(index);
            }
            onItemCollected.Invoke(); // destroy unselected items
        }

        private ItemPickup SpawnItem(ItemPickup item, Vector3 location) {
            ItemPickup newItem = Instantiate(item, location, Quaternion.identity);
            newItem.gameObject.SetActive(false);
            StartCoroutine(ThrowItemFromDoor(newItem, itemSpawnLocation.position, location, itemThrowDuration));
            return newItem;
        }
        private ItemPickup MakeValidItem(ItemType excludeType, ItemRarity minRarity, Vector3 location) {
            if (pickups.Count == 0) return null;

            int startingIndex = Random.Range(0, pickups.Count);
            int i = startingIndex;
            ItemPickup p = null;
            ItemPickup skippedRarerItem = null;
            while(p == null) {
                if (pickups[i].itemType != excludeType && pickups[i].itemRarity >= minRarity) {
                    if(pickups[i].itemRarity == minRarity) {
                        p = pickups[i];
                    }
                    else {
                        int pickupChance = 0;
                        switch (pickups[i].itemRarity) {
                            case ItemRarity.UNCOMMON:
                                pickupChance = 2; //one in two chance
                                break;
                            case ItemRarity.RARE:
                                pickupChance = 4;
                                break;
                        }

                        int doSpawn = Random.Range(0, pickupChance);
                        if (doSpawn == 0) {
                            p = pickups[i];
                        }
                        else {
                            skippedRarerItem = pickups[i];
                        }
                    }
                }
                if(p == null) {
                    i++;
                    if (i >= pickups.Count) i = 0;
                    if (i == startingIndex) return skippedRarerItem;
                }
            }
            ItemPickup item = SpawnItem(p, location);
            item.SetIndex(i);
            return p;
        }
        
        
        IEnumerator ThrowItemFromDoor(ItemPickup item, Vector3 startPos, Vector3 endPos, float duration) {
            yield return new WaitForSeconds(.5f);
            
            item.gameObject.SetActive(true);
            
            
            float time = 0;
            
            while (time < duration) {
                time += Time.deltaTime;
                Vector3 pos = Vector3.Lerp(startPos, endPos, time / duration);
                pos.y = throwHeightCurve.Evaluate(time / duration);
                item.transform.position = pos;
                yield return null;
            }
            
            item.StartUp();
        }

        #region Serialization
            public string SerializeItemList() {
                string output = "";
                foreach (ItemPickup i in permanentItemPool) {
                    int index = listOfAllItems.IndexOf(i);
                    output += index;
                    output +=";";
                }
                return output;
            }

            public void DeserializeItemList(string input) {
                if (input == "" || input == "{}") return;

                Debug.Log("deserializing: " + input);
                string[] inputs = input.Split(';');
                List<ItemPickup> tempItemList = new List<ItemPickup>();
                foreach (string i in inputs) {
                    if (i != "") {
                        int index = int.Parse(i);
                        tempItemList.Add(listOfAllItems[index]);
                    }
                }
                if (tempItemList.Count > 0) {
                    permanentItemPool = tempItemList.ToList();
                }
            }
        #endregion
    }
    
}
