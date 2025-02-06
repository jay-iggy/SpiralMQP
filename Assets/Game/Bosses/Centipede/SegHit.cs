using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Interfaces;
using Game.Scripts;

// Segment Hit
public class SegHit : MonoBehaviour, ICanGetHit
{
    private HealthComponent healthComponent;

    public void Start()
    {
        healthComponent = GetComponentInParent<HealthComponent>();

        if (healthComponent == null)
            Debug.Log("NO HC FOUND");
    }

    public void GetHit(float damage, bool overrideInvincibility = false)
    {
        healthComponent.TakeDamage(damage);

        Debug.Log("Got hit");
    }

    public bool CanBeHit(bool overrideInvincibility = false)
    {
        return true;
    }
}
