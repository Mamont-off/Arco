using Unity.Entities;

namespace Player
{
    public struct PlayerInputComponent : IComponentData
    {
        public bool LeftPressed;
        public bool RightPressed;
        public bool ShootPressed;
    }
}