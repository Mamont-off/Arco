using Components;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct BulletTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletComponent>();
            state.RequireForUpdate<EnemyComponent>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var bulletEvent = new BulletTriggerEventJob
            {
                EnemyComponentLookup = SystemAPI.GetComponentLookup<EnemyComponent>(false),
                BulletComponentLookup = SystemAPI.GetComponentLookup<BulletComponent>(false),
                Ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().
                    ValueRW.CreateCommandBuffer(state.WorldUnmanaged)
            };
            state.Dependency = bulletEvent.Schedule(simulation, state.Dependency);
        }
        
        [BurstCompile]
        private partial struct BulletTriggerEventJob : ITriggerEventsJob
        {
            internal ComponentLookup<EnemyComponent> EnemyComponentLookup;
            internal ComponentLookup<BulletComponent> BulletComponentLookup;
            
            internal EntityCommandBuffer Ecb;
            
            public void Execute(TriggerEvent collisionEvent)
            {
                if (!EnemyComponentLookup.HasComponent(collisionEvent.EntityA))
                {
                    return;
                }

                var enemyEntity = collisionEvent.EntityA;
                var bulletEntity = collisionEvent.EntityB;
                
                Ecb.SetEnabled(bulletEntity, false);
                var damage = BulletComponentLookup.GetRefRO(bulletEntity).ValueRO.Damage;
                var currentHp = (EnemyComponentLookup.GetRefRW(enemyEntity).ValueRW.CurrentHP -= damage);
                if (currentHp <= 0)
                {
                    Ecb.DestroyEntity(enemyEntity);
                }
            }
        }
        
    }
}