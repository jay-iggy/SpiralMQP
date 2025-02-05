using Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsAbility : MonoBehaviour
{
    private GameObject player;
    private HealthComponent playerHp;
    private HealthComponent bossHp;
    [SerializeField] float dmg = 5;
    void Start()
    {
        player = transform.parent.parent.gameObject;
        playerHp = player.GetComponent<HealthComponent>();
        playerHp.onTakeDamage.AddListener(TriggerThorns);
        CombatManager.instance.onBossSpawned.AddListener(SetBoss);
    }

    public void SetBoss()
    {
        bossHp = CombatManager.instance.currentBoss.GetComponent<HealthComponent>();
    }

    public void TriggerThorns()
    {
        bossHp.TakeDamage(dmg, true);
    }
}
