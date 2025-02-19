using UnityEngine;

namespace Game.Scripts.Interfaces
{
    public interface IHaveOneAttack
    { 
        public float Attack(); //returns length of attack
        public void StopAttacking();
        public void SetBullet(GameObject b);
    }
}