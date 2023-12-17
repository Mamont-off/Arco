using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EndGameSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FinishGameRule>().Build());
        }
        
        [BurstCompile]
        protected override void OnUpdate()
        {
            var fgr = SystemAPI.GetSingleton<FinishGameRule>();
            if (fgr.IsGameEnd())
            {
                Quit();
            }
        }
        
        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}