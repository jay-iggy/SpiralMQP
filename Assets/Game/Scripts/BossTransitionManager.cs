using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using TMPro;
using UnityEngine;

public class BossTransitionManager : MonoBehaviour {
    public static BossTransitionManager instance;
    
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public HealthBar healthBar;
    public TextMeshProUGUI bossNameTextObject;

    
    public void SpawnBoss(EnemyData enemyData, out Boss boss) {
        boss = Instantiate(enemyData.bossPrefab);
        SetBossUI(boss.GetComponent<HealthComponent>(), enemyData.enemyName);
    }
    public void SetBossUI(HealthComponent bossHealthComponent, string bossName) {
        healthBar.BindHealthComponent(bossHealthComponent);
        bossNameTextObject.text = bossName;
    }
}
