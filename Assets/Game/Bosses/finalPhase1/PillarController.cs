using Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class PillarController : Boss
    {

        protected override void SetBossDefeated()
        {
            //don't
        }

        public override void Die()
        {
            Destroy(transform.parent.gameObject);
        }

    }
}
