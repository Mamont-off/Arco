using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Player.System
{
    [BurstCompile]
    public partial struct CannonSystem : ISystem
    {
        [ReadOnly] private ComponentLookup<PlayerInputComponent> _inputLookup;
        private EntityQuery _disabledBullets;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _inputLookup = state.GetComponentLookup<PlayerInputComponent>();
            
            var disabledComps = new NativeArray<ComponentType>(2, Allocator.Temp)
            {
                [0] = ComponentType.ReadWrite<BulletComponent>(),
                [1] = ComponentType.ReadOnly<Disabled>()
            };
            _disabledBullets = state.GetEntityQuery(disabledComps);
            
            state.RequireForUpdate<CannonComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _inputLookup.Update(ref state);
            
            foreach (var (cannonComp, parent)
                     in SystemAPI.Query<
                         RefRW<CannonComponent>,
                         RefRO<Parent>>())
            {
                cannonComp.ValueRW.CurrentTime = 
                    math.clamp(cannonComp.ValueRW.CurrentTime + SystemAPI.Time.DeltaTime,
                        0, cannonComp.ValueRO.ShootTime);
                
                if (!cannonComp.ValueRO.IsReloadToOver && cannonComp.ValueRW.IsReady)
                {
                    cannonComp.ValueRW.IsReady = false;
                }

                if (!_inputLookup.HasComponent(parent.ValueRO.Value))
                {
                    continue;
                }
                
                if (_inputLookup[parent.ValueRO.Value].ShootPressed)
                {
                    if (cannonComp.ValueRO.IsReloadToOver)
                    {
                        cannonComp.ValueRW.IsReady = true;
                    }
                    
                    if (_disabledBullets.CalculateEntityCount() <= 1)
                    {
                        SystemAPI.GetSingletonRW<BulletSpawnComponent>().ValueRW.SpawnCount = 5;
                    }
                }
            }
        }
    }
}