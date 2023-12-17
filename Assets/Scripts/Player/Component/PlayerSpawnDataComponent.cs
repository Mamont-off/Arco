using Unity.Entities;
using Unity.Mathematics;

namespace Player
{
    public struct PlayerSpawnDataComponent : IComponentData
    {
        public Entity Player;
        public float Speed;
        public float ShootSpeed;
        public float3 SpawnPosition;
        public float MoveBorder;
    }
}