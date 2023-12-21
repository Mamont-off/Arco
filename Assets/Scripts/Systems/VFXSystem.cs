using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct VFXSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(SystemAPI.QueryBuilder().
                WithAny<VFXDataComponent, VFXComponent, SpawnVFXComponent>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new SpawnVFXJob
            {
                VfxData = SystemAPI.GetSingleton<VFXDataComponent>(),
                Ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(state.Dependency);
            
            var particleJob = new VFXLifeTimeJob
            {
                
                DeltaTime = SystemAPI.Time.DeltaTime,
                ECB = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW.
                    CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            };
            state.Dependency = particleJob.ScheduleParallel(state.Dependency);

        }

        [BurstCompile]
        private partial struct SpawnVFXJob : IJobEntity
        {
            internal VFXDataComponent VfxData;
            internal EntityCommandBuffer Ecb;
            private void Execute(Entity entity, SpawnVFXComponent spawnComp)
            {
                var particle = Ecb.Instantiate(VfxData.ExplosionVFX);
                Ecb.AddComponent(particle, new VFXComponent(VfxData.Lifetime));
                Ecb.SetComponent(particle, LocalTransform.FromPosition(spawnComp.Position));
                
                Ecb.RemoveComponent<SpawnVFXComponent>(entity);
            }
        }
        
        [BurstCompile]
        public partial struct VFXLifeTimeJob : IJobEntity
        {
            internal float DeltaTime;
            internal EntityCommandBuffer.ParallelWriter ECB;
        
            private void Execute(Entity entity, [ChunkIndexInQuery] int index, ref VFXComponent vfxComp)
            {
                vfxComp.LifeTime -= DeltaTime;
                
                if (vfxComp.LifeTime <= 0f)
                {
                    ECB.DestroyEntity(index, entity);
                }
            }
        }
    }
}