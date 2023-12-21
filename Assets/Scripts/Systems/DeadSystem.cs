using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    /// <summary> System for destroy entity with DeadTag </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DeadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DeadTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (tag, entity) in 
                     SystemAPI.Query<DeadTag>()
                         .WithNone<SpawnVFXComponent>().WithDisabled<PlaySFXComponent>()
                         .WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}