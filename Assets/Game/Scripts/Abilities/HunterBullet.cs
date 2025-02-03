using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    public class HunterBullet : Projectile
    {
        //Like a normal bullet, but deals extra damage to low health targets
        [SerializeField] float vulnerableHP = 30;       
        protected override void OnHitTarget(ICanGetHit target)
        {
            HealthComponent hc = target as HealthComponent;
            int addDamage = 0;
            if (hc.health < vulnerableHP)
            {
                addDamage = 4;
            }
            target.GetHit(dmg+addDamage, ignoresInvincibility);
            if (!persistent) DestroySelf();
        }
    }
}
