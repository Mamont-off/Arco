using Unity.Entities;

namespace Player
{
    public struct BulletComponent : IComponentData
    {
        public readonly Entity Cannon;
        public int Damage;
        public float Speed;
        public readonly float DeadDistance; 

        public BulletComponent(Entity cannon, int damage, float speed, float deadDistance)
        {
            Damage = damage;
            Speed = speed;
            DeadDistance = deadDistance;
            Cannon = cannon;
        }
    }
}