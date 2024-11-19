using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace SF.EntitiesModule
{
    partial struct SelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach(RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);

                visualLocalTransform.ValueRW.Scale = selected.ValueRO.ShowScale;
            }


            foreach(RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);

                visualLocalTransform.ValueRW.Scale = 0;
            }
        }
    }
}
