using Other;
using Unity.Entities;

namespace Components
{
    public struct PlaySFXComponent : IComponentData, IEnableableComponent
    {
        public SoundType Type;
    }
}