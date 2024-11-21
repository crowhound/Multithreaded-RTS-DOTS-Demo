using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

using SF.EntitiesModule.Combat;

namespace SF.EntitiesModule
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthDeadSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // We get a command buffer to set up entity destruction when an entity dies.
            // We can use one of the Simulation ones to allow choosing when the command buffer runs.
            EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


            // The WithEntityAccess allows us to get the entity the components are attached during the query.
            // The Entity declaration has to be the last variable.
            foreach((
                RefRO<Health> health, 
                Entity entity) in SystemAPI.Query
                    <RefRO<Health>>().WithEntityAccess())
            {
                // Is the entity dead
                if(health.ValueRO.HealthAmount <= 0)
                {
                    // Entity is dead. Set up a command in the entityCommandBuffer to destroy it out of the loop.
                    // You can not change structural data of entities during a query loop so we have to tell it
                    // to do an entityCommandBuffer to destroy it later.
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}
