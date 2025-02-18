using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class PillarController : Boss
    {
        public FinalPhase1Attacks mainBoss;

        protected override void SetBossDefeated()
        {
            //don't
        }

        public override void Die()
        {
            mainBoss.KillPillar();
            Destroy(transform.parent.gameObject);
        }

    }
}
