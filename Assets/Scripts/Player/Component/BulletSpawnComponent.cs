using Unity.Entities;

namespace Player
{
    public struct BulletSpawnComponent : IComponentData
    {
        public Entity Bullet;
        public int SpawnCount;
        public int Damage;
        public float Speed;
        public int DestroyDistance;
    }
}