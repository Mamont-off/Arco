using Unity.Entities;

namespace Components
{
    public struct BuffComponent : IComponentData
    {
        public float Speed;
        public float SpeedStep;
        public float YStep;
    }
}