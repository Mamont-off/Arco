using Other;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct SpawnVFXComponent : IComponentData
    {
        public readonly VFXType Type;
        public readonly float3 Position;

        public SpawnVFXComponent(VFXType type, float3 position)
        {
            Type = type;
            Position = position;
        }
    }
}