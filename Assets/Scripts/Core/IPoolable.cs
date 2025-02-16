public interface IPoolable
{
    void Create();
    void Spawn();
    void Despawn();
    void OnDespawn();
}