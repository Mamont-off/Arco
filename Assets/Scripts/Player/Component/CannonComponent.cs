using Unity.Entities;

namespace Player
{
    public struct CannonComponent : IComponentData
    {
        public bool IsReady;
        public float CurrentTime;
        public float ShootTime;

        public CannonComponent(float shootTime)
        {
            ShootTime = shootTime;
            CurrentTime = 0;
            IsReady = false;
        }

        public bool IsReloadToOver => CurrentTime >= ShootTime;
    }
}