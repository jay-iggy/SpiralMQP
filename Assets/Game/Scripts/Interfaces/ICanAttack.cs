namespace Game.Scripts.Interfaces {
    public interface ICanAttack { // class needs a more descriptive name (i.e. IAttackSet), ICanAttack is not accurate because this class represents a collection of attacks
        /// <summary>
        /// Use attack of the given index from set of attacks
        /// </summary>
        /// <returns>Duration of attack</returns>
        public float Attack(int index); //returns length of attack
        public int GetAttackCount();
    }
}