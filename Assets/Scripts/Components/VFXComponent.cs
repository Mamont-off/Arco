using Unity.Entities;

namespace Components
{
    public struct VFXComponent : IComponentData
    {
        public float LifeTime;

        public VFXComponent(float lifeTime)
        {
            LifeTime = lifeTime;
        }
    }
}