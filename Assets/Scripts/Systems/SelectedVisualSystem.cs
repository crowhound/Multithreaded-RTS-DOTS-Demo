using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace SF.EntitiesModule
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventSystem))]
    partial struct SelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach(RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                // Only update the visuals during the frame one of the events were going off.
                if(selected.ValueRO.OnSelected)
                {
                    RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
                    visualLocalTransform.ValueRW.Scale = selected.ValueRO.ShowScale;
                }
                if(selected.ValueRO.OnDeselected)
                {
                    RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
                    visualLocalTransform.ValueRW.Scale = 0;
                }
            }
        }
    }
}
