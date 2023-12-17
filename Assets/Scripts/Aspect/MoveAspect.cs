using Components;
using Unity.Entities;
using Unity.Transforms;

namespace Aspect
{
    public readonly partial struct MoveAspect : IAspect
    {
        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRO<BorderComponent> BorderComp;
        public readonly RefRW<SpeedComponent> SpeedComp;
        public readonly RefRO<BuffComponent> BuffComp;
    }
}