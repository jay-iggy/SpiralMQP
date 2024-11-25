namespace Game.Scripts.Interfaces {
    public interface ICanGetHit {
        public void GetHit(float damage, bool overrideInvincibility = false);
        public bool CanBeHit(bool overrideInvincibility = false);
    }
}