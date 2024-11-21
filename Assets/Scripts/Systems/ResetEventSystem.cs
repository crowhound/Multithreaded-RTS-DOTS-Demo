using Unity.Burst;
using Unity.Entities;

namespace SF.EntitiesModule
{
    /*  Makes sure the event runs in the last update group to gurantee the UnitMover 
     *  and SelectedVisual systems run before it. Without doing this there is a chance this willl run before the 
     *  either of them making certain events misfire at wrong types or not fire at all.
     */
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ResetEventSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Reset events on any entity that has a selected component even if it isn;t selected.
            foreach(RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.OnSelected = false;
                selected.ValueRW.OnDeselected = false;
            }
        }
    }
}
