using Aspect;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpeedComponent>();
            state.RequireForUpdate<BorderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var moveAspect in SystemAPI.Query<MoveAspect>())
            {
                var newXPos = moveAspect.Transform.ValueRO.Position.x + 
                              ((moveAspect.SpeedComp.ValueRO.Speed+moveAspect.BuffComp.ValueRO.Speed) 
                               * moveAspect.SpeedComp.ValueRO.MoveDirection) 
                               * SystemAPI.Time.DeltaTime;
                moveAspect.Transform.ValueRW.Position.x = 
                    math.clamp(newXPos, moveAspect.BorderComp.ValueRO.Left, moveAspect.BorderComp.ValueRO.Right);
            }

        }
    }
}