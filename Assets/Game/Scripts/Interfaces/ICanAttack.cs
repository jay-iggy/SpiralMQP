namespace Game.Scripts.Interfaces {
    public interface ICanAttack { // class needs a more descriptive name (i.e. IAttackSet), ICanAttack is not accurate because this class represents a collection of attacks
        public float Attack(int index); //returns length of attack
        public int GetAttackCount();
    }
}