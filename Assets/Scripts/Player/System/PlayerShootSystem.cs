using Components;
using Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Player.System
{
    [BurstCompile]
    public partial struct PlayerShootSystem : ISystem
    {
        [ReadOnly] private ComponentLookup<CannonComponent> _cannonComp;
        [ReadOnly] private ComponentLookup<LocalToWorld> _cannonPosition;
        private ComponentLookup<PlaySFXComponent> _cannonSfxComp;
        
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _cannonComp = state.GetComponentLookup<CannonComponent>();
            _cannonPosition = state.GetComponentLookup<LocalToWorld>();
            _cannonSfxComp = state.GetComponentLookup<PlaySFXComponent>();
            
            var disabledComps = new NativeArray<ComponentType>(2, Allocator.Temp)
            {
                [0] = ComponentType.ReadWrite<BulletComponent>(),
                [1] = ComponentType.ReadWrite<Disabled>()
            };
            
            state.RequireForUpdate(state.GetEntityQuery(disabledComps));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _cannonComp.Update(ref state);
            _cannonPosition.Update(ref state);
            _cannonSfxComp.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (bulletComp, transformComp, entity)
                     in SystemAPI.Query<
                         RefRW<BulletComponent>,
                         RefRW<LocalTransform>>().
                         WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                if (!SystemAPI.HasComponent<Disabled>(entity))
                {
                    continue;
                }
                
                if (!_cannonComp.HasComponent(bulletComp.ValueRO.Cannon))
                {
                    continue;
                }

                if (_cannonComp.GetRefRO(bulletComp.ValueRO.Cannon).ValueRO.IsReady)
                {
                    if (!_cannonSfxComp.IsComponentEnabled(bulletComp.ValueRO.Cannon))
                    {
                        ecb.SetComponentEnabled<PlaySFXComponent>(bulletComp.ValueRO.Cannon, true);
                    }
                    
                    var canonComp = _cannonComp.GetRefRW(bulletComp.ValueRO.Cannon);
                    canonComp.ValueRW.CurrentTime = 0;
                    canonComp.ValueRW.IsReady = false;
                    
                    transformComp.ValueRW.Position =
                        _cannonPosition.GetRefRO(bulletComp.ValueRO.Cannon).ValueRO.Position;
                    
                    ecb.SetEnabled(entity, true);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
    
}