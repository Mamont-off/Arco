using Unity.Entities;

namespace Components
{
    public struct FinishGameRule : IComponentData
    {
        public int EnemyCount;
        public float EnemyHeight;
        public float DeadHeight;

        public bool IsGameEnd()
        {
            if (EnemyCount == 0)
            {
                return true;
            }

            if (EnemyHeight <= DeadHeight)
            {
                return true;
            }

            return false;
        }
    }
}