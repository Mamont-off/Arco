using Unity.Entities;

namespace Components
{
    public readonly struct BorderComponent : IComponentData
    {
        public readonly float Right;
        public readonly float Left;

        public BorderComponent(float right) => Left = -(Right = right);
    }
}