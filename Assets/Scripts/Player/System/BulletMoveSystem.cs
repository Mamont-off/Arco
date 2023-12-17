using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Player.System
{
    [BurstCompile]
    public partial struct BulletMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (bulletComp, transformComp, entity)
                     in SystemAPI.Query<
                             RefRW<BulletComponent>,
                             RefRW<LocalTransform>>().WithEntityAccess()
                         .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
                     )
            {
                if (SystemAPI.HasComponent<Disabled>(entity))
                {
                    continue;
                }

                if (transformComp.ValueRO.Position.y <= bulletComp.ValueRO.DeadDistance)
                {
                    var newYPos = transformComp.ValueRO.Position.y +
                                  (bulletComp.ValueRO.Speed * SystemAPI.Time.DeltaTime);
                    transformComp.ValueRW.Position.y = newYPos;
                }
                else
                {
                    ecb.SetEnabled(entity, false);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}