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
    public List<EnemyData> nextEnemies = new List<EnemyData>();
}
