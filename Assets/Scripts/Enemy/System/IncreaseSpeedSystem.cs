using Components;
using Unity.Burst;
using Unity.Entities;

namespace Enemy.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class IncreaseSpeedSystem : SystemBase
    {
        private int _totalCount;
        private int _lastCount;
        private EntityQuery _enemys;
        
        [BurstCompile]
        protected override void OnCreate()
        {
            _enemys = SystemAPI.QueryBuilder().WithAll<EnemyComponent, BuffComponent>().Build();
        }
        
        [BurstCompile]
        protected override void OnUpdate()
        {
            var currentCountOfEnemy = _enemys.CalculateEntityCount();
            if (_totalCount < currentCountOfEnemy)
            {
                _totalCount = _lastCount = currentCountOfEnemy;
            }
            if (_lastCount > currentCountOfEnemy)
            {
                _lastCount = currentCountOfEnemy;

                new BuffIncreaseSpeedJob(){TotalCount = _totalCount, LastCount = _lastCount}.ScheduleParallel(_enemys);

                SystemAPI.GetSingletonRW<FinishGameRule>().ValueRW.EnemyCount = _lastCount;
            }
        }
        
        [BurstCompile]
        partial struct BuffIncreaseSpeedJob : IJobEntity
        {
            internal int TotalCount;
            internal int LastCount;
            [BurstCompile]
            private void Execute(ref BuffComponent buffComp)
            {
                buffComp.Speed = (buffComp.SpeedStep * (TotalCount - LastCount));
            }
        }
    }
}