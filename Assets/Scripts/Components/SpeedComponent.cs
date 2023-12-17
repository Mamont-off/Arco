using Unity.Entities;

namespace Components
{
    public struct SpeedComponent : IComponentData
    {
        public float BaseSpeed;
        public float Speed;
        public int MoveDirection;

        public SpeedComponent(float baseSpeed)
        {
            BaseSpeed = Speed = baseSpeed;
            MoveDirection = 1;
        }
        public void SwapMoveDirection()
        {
            MoveDirection = -MoveDirection;
        }
    }
}