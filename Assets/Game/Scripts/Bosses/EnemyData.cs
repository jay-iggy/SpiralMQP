using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

/// <summary>
/// This scriptable object holds the data for each enemy, including the name and prefab
/// </summary>
[CreateAssetMenu(fileName = "EnemyData")]
public class EnemyData : ScriptableObject {
    public string enemyName;
    public Boss bossPrefab;
    public string bossMusic = "rat_OST";
    public int bossIndex=-1; //used to track which bosses have been defeated before
    [Header("Items")]
    public ItemRarity minItemRarity = ItemRarity.COMMON;
    public List<ItemPickup> unlockedItems;
    
    
    // DEPRECATED
    [HideInInspector]public List<EnemyData> nextEnemies = new List<EnemyData>();
}
