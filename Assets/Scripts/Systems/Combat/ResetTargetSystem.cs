using Unity.Burst;
using Unity.Entities;

namespace SF.EntitiesModule
{
    /// <summary>
    /// Resets targets on entities if their target was destroyed in memory to
    /// prevent acccesing memory objects that no longer exist.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ResetTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach(RefRW<Target> target in 
                        SystemAPI.Query
                            <RefRW<Target>>())
            {
                // If the target has been destroyed in any other system/job.
                // reset it to a new null entity prevent trying to get data on a destroyed IComponentData.
                if(!SystemAPI.Exists(target.ValueRO.TargetEntity))
                {
                    target.ValueRW.TargetEntity = Entity.Null;
                }

            }
        }
    }
}
