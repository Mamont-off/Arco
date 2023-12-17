using Unity.Entities;

namespace Components
{
    public struct EnemyComponent : IComponentData
    {
        public int InitHP;
        public int CurrentHP;

        public EnemyComponent(int initHP)
        {
            InitHP = initHP;
            CurrentHP = initHP;
        }
    }
}