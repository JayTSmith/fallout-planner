namespace TabletopSimulator.Combat {
    internal interface ISpawn
    {
        public void Spawn(bool despawn = false);
        public void Despawn();
    }
}