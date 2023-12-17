using Unity.Entities;

namespace Components
{
    public struct EnemySpawnDataComponent : IComponentData
    {
        public Entity Enemy;
        public float BaseSpeed;
        public int Hp;
        public float IncreaseSpeedBy;
        public float EnemyXOffset;
        public float EnemyYOffset;
        public int RowCount;
        public float MoveBorder;
        public float MaxHeight;
    }
}