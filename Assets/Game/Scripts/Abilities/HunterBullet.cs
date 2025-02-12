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
            float projDmg = dmg;
            bool isCrit = false;
            
            // Combo: Every 3 consecutive hits, the damage is doubled
            if(projID != -1) {
                hitIDs.Add(projID);
                if(IsCombo()) {
                    projDmg *= 2;
                    hitIDs.Clear();
                    isCrit = true;
                }
            }
            
            HealthComponent hc = target as HealthComponent;
            if (hc != null && hc.health < vulnerableHP) {
                projDmg *= 2;
            }
            target.GetHit(projDmg, ignoresInvincibility, isCrit);
            if (!persistent) DestroySelf();
        }
    }
}
