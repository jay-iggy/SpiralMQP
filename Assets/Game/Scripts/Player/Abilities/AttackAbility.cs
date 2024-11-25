using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public abstract class AttackAbility : Ability
    {
        public abstract void ModifyDamage(float delta);
    }
}
