using Unity.Entities;

namespace Components
{
    public struct VFXDataComponent : IComponentData
    {
        public Entity ExplosionVFX;
        public float Lifetime;
    }
}