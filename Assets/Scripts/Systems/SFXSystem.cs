using Components;
using Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    public partial class SFXSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<PlaySFXComponent>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (playSfxComp, entity) in
                     SystemAPI.Query<RefRO<PlaySFXComponent>>().WithEntityAccess())
            {
                switch (playSfxComp.ValueRO.Type)
                {
                    case SoundType.Explosion:
                        AudioManager.Instance.PlayExplosion();
                        break;
                    
                    default:
                        AudioManager.Instance.PlayShoot();
                        break;
                }
                ecb.SetComponentEnabled<PlaySFXComponent>(entity, false);
            }
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}