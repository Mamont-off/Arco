using Components;
using Unity.Burst;
using Unity.Entities;

namespace Player.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PlayerInputSystem : SystemBase
    {
        private DefaultInput.DefaultActions _defaultInput;

        [BurstCompile]
        protected override void OnCreate()
        {
            var inputActions = new DefaultInput();
            inputActions.Enable();
            _defaultInput = inputActions.Default;
        
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build());
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            foreach (var (playerInput, speedComp) in 
                     SystemAPI.Query<RefRW<PlayerInputComponent>, RefRW<SpeedComponent>>())
            {
                var leftPressed = playerInput.ValueRW.LeftPressed = _defaultInput.Left.IsPressed();
                var rightPressed = playerInput.ValueRW.RightPressed = _defaultInput.Right.IsPressed();
                playerInput.ValueRW.ShootPressed = _defaultInput.Shoot.IsPressed();
                
                if (leftPressed && speedComp.ValueRO.MoveDirection > 0)
                {
                    speedComp.ValueRW.SwapMoveDirection();
                }

                if (rightPressed && speedComp.ValueRO.MoveDirection < 0)
                {
                    speedComp.ValueRW.SwapMoveDirection();
                }

                if (leftPressed || rightPressed)
                {
                    speedComp.ValueRW.Speed = speedComp.ValueRO.BaseSpeed;
                }
                else
                {
                    speedComp.ValueRW.Speed = 0;
                }
            }
        }
    }
}