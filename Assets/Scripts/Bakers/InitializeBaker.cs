using Autoring;
using Components;
using Player;
using Unity.Burst;
using Unity.Entities;

namespace Bakers
{
    [BurstCompile]
    public class InitializeBaker : Baker<InitAuthoring>
    {
        [BurstCompile]
        public override void Bake(InitAuthoring authoring)
        {
            var initEntity = GetEntity(TransformUsageFlags.None);
            AddComponent<SpawnEntityTag>(initEntity);
        
            var enemyPrefab = authoring.EnemyData.Prefab;
            var enemyEntity = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);
            AddComponent(initEntity, new EnemySpawnDataComponent
            {
                Enemy = enemyEntity,
                Hp = authoring.EnemyData.Hp,
                BaseSpeed = authoring.EnemyData.BaseMoveSpeed,
                RowCount = authoring.EnemyData.RowCount,
                EnemyXOffset = authoring.EnemyData.EnemyXOffset,
                EnemyYOffset = authoring.EnemyData.EnemyYOffset,
                MoveBorder = authoring.EnemyData.MoveBorder,
                MaxHeight = authoring.EnemyData.MaxHeight,
                IncreaseSpeedBy = authoring.EnemyData.IncreaseSpeedStep
            });

            var playerPrefab = authoring.PlayerData.Prefab;
            var playerEntity = GetEntity(playerPrefab, TransformUsageFlags.Dynamic);
            AddComponent(initEntity, new PlayerSpawnDataComponent
            {
                Player = playerEntity, 
                Speed = authoring.PlayerData.Speed, 
                ShootSpeed = authoring.BulletData.ShootSpeed,
                SpawnPosition = authoring.PlayerData.SpawnPosition,
                MoveBorder = authoring.PlayerData.MoveBorder
            });

            var bulletPrefab = authoring.BulletData.Prefab;
            var bulletEntity = GetEntity(bulletPrefab, TransformUsageFlags.Dynamic);
            AddComponent(initEntity, new BulletSpawnComponent()
            {
                Bullet = bulletEntity,
                Damage = authoring.BulletData.Damage,
                DestroyDistance = authoring.BulletData.DestroyDistance,
                SpawnCount = 5,
                Speed = authoring.BulletData.BulletSpeed
            });
            
            AddComponent(initEntity, new FinishGameRule
            {
                DeadHeight = authoring.InitializeData.DeathHeigh,
                EnemyHeight = authoring.EnemyData.MaxHeight,
                EnemyCount = 1+authoring.EnemyData.RowCount
            });
        }
    }
}
