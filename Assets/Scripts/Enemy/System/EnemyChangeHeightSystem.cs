using Aspect;
using Components;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Enemy.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EnemyChangeHeightSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<EnemyComponent>().Build());
        }
        
        [BurstCompile]
        protected override void OnUpdate()
        {
            bool moveToNextHeight = false;
            foreach (var (localTransform, borderComp) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<BorderComponent>>().WithNone<PlayerTag>())
            {
                if (localTransform.ValueRO.Position.x >= borderComp.ValueRO.Right ||
                    localTransform.ValueRO.Position.x <= borderComp.ValueRO.Left)
                {
                    moveToNextHeight = true;
                    break;
                }
            }

            if (moveToNextHeight)
            {
                var lowest = float.MaxValue;
                foreach (var moveAspect in SystemAPI.Query<MoveAspect>().WithNone<PlayerTag>())
                {
                    moveAspect.SpeedComp.ValueRW.SwapMoveDirection();
                    var yPos = (moveAspect.Transform.ValueRW.Position.y -= moveAspect.BuffComp.ValueRO.YStep);
                    if (lowest > yPos)
                    {
                        lowest = yPos;
                    }
                }
                
                SystemAPI.GetSingletonRW<FinishGameRule>().ValueRW.EnemyHeight = lowest;
            }
        }
    }
}