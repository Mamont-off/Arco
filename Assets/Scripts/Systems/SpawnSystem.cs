using Components;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct SpawnSystem : ISystem
    {
        private const int DefaultEnemyRotate = 80;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnEntityTag>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (spawnData, entity) 
                     in SystemAPI.Query<
                             RefRO<PlayerSpawnDataComponent>>()
                         .WithEntityAccess())
            {
                var playerInstance = ecb.Instantiate(spawnData.ValueRO.Player);
                ecb.AddComponent(playerInstance, new PlayerInputComponent());
                ecb.AddComponent(playerInstance, new SpeedComponent(spawnData.ValueRO.Speed));
                ecb.AddComponent(playerInstance, new BuffComponent());
                ecb.AddComponent(playerInstance, new BorderComponent(spawnData.ValueRO.MoveBorder));
                ecb.AddComponent(playerInstance, new PlayerTag());
                
                ecb.SetComponent(playerInstance, 
                    LocalTransform.FromPosition(spawnData.ValueRO.SpawnPosition));
                
                ecb.RemoveComponent<PlayerSpawnDataComponent>(entity);

                var cannonComp = new CannonComponent(spawnData.ValueRO.ShootSpeed);
                var parentComp = new Parent
                {
                    Value = playerInstance
                };
                
                var cannonEntity = ecb.CreateEntity();
                ecb.AddComponent(cannonEntity, cannonComp);
                ecb.AddComponent(cannonEntity, parentComp);
                ecb.AddComponent(cannonEntity, new LocalToWorld());
                ecb.AddComponent(cannonEntity, LocalTransform.Identity);
            }

            foreach (var (spawnData, entity)
                     in SystemAPI.Query<
                             RefRO<EnemySpawnDataComponent>>()
                         .WithEntityAccess())
            {
                var nextRowPosX = spawnData.ValueRO.MoveBorder-spawnData.ValueRO.EnemyXOffset;
                var endRowY = spawnData.ValueRO.MaxHeight - 
                              spawnData.ValueRO.EnemyYOffset * spawnData.ValueRO.RowCount;
                var pos = new float3(-nextRowPosX, spawnData.ValueRO.MaxHeight,0);
                var currentPos = 
                    LocalTransform.FromPositionRotation(pos, quaternion.RotateY(DefaultEnemyRotate));
                
                while (currentPos.Position.y > endRowY)
                {
                    var enemyInstance = ecb.Instantiate(spawnData.ValueRO.Enemy);
                    ecb.AddComponent(enemyInstance, new EnemyComponent(spawnData.ValueRO.Hp));
                    ecb.AddComponent(enemyInstance, new SpeedComponent(spawnData.ValueRO.BaseSpeed));
                    ecb.AddComponent(enemyInstance, new BorderComponent(spawnData.ValueRO.MoveBorder));
                    ecb.AddComponent(enemyInstance, new BuffComponent
                    {
                        Speed = 0,
                        SpeedStep = spawnData.ValueRO.IncreaseSpeedBy,
                        YStep = spawnData.ValueRO.EnemyYOffset
                    });
                    ecb.AddComponent(enemyInstance, new LocalToWorld());
                    ecb.SetComponent(enemyInstance, currentPos);
                    
                    currentPos.Position.x += spawnData.ValueRO.EnemyXOffset;
                    
                    if (currentPos.Position.x > nextRowPosX)
                    {
                        currentPos.Position.x = pos.x;
                        currentPos.Position.y -= spawnData.ValueRO.EnemyYOffset;
                    }
                    
                }
                
                ecb.RemoveComponent<EnemySpawnDataComponent>(entity);
            }


            foreach (var bulletSpawnComp
                     in SystemAPI.Query<RefRW<BulletSpawnComponent>>())
            {
                if (bulletSpawnComp.ValueRO.SpawnCount <= 0)
                {
                    continue;
                }

                if (SystemAPI.TryGetSingletonEntity<CannonComponent>(out var canon))
                {
                    if (bulletSpawnComp.ValueRO.SpawnCount > 0)
                    {
                        state.Dependency = new CreateBulletsJob()
                        {
                            Ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().
                                ValueRW.CreateCommandBuffer(state.WorldUnmanaged),
                            BulletPrefab = bulletSpawnComp.ValueRO.Bullet,
                            Cannon = canon,
                            Damage = bulletSpawnComp.ValueRO.Damage,
                            Speed = bulletSpawnComp.ValueRO.Speed,
                            DestroyDistance = bulletSpawnComp.ValueRO.DestroyDistance,
                            Count = bulletSpawnComp.ValueRO.SpawnCount
                        }.Schedule(state.Dependency);
                        
                        bulletSpawnComp.ValueRW.SpawnCount = 0;
                    }
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            
        }

        [BurstCompile]
        private partial struct CreateBulletsJob : IJobEntity
        {
            internal EntityCommandBuffer Ecb;
            
            internal Entity Cannon;
            internal Entity BulletPrefab;
            internal int Damage;
            internal float Speed;
            internal float DestroyDistance;

            internal int Count;

            private void Execute()
            {
                while (Count-- > 0)
                {
                    var bulletEntity = Ecb.Instantiate(BulletPrefab);
                    Ecb.AddComponent(bulletEntity, new BulletComponent(
                        Cannon, Damage, Speed, DestroyDistance));
                    
                    Ecb.SetEnabled(bulletEntity, false);
                }
            }
        }
    }
}