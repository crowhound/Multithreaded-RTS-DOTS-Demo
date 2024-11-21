using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;


namespace SF.EntitiesModule
{
    partial struct FindTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton pysicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = pysicsWorldSingleton.CollisionWorld;

            NativeList<DistanceHit> distanceHitsList = new NativeList<DistanceHit>(Allocator.Temp);
        
            foreach((
                RefRO<LocalTransform> localTransform, 
                RefRW<FindTarget> findTarget,
                RefRW<Target> target)
                in SystemAPI.Query<
                    RefRO<LocalTransform>, 
                    RefRW<FindTarget>,
                    RefRW<Target>>())
            {
                findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if(findTarget.ValueRO.Timer > 0)
                {
                    // Timer has not elasped 
                    continue;
                }
                findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;

                // Reset the hit list just in case we hit some last frame.
                // If we don't we could have ghost collision hits.
                distanceHitsList.Clear();

                // Look for entities on the units physics layer.
                if(collisionWorld.OverlapSphere(
                    localTransform.ValueRO.Position, 
                    findTarget.ValueRO.SearchRange, 
                    ref distanceHitsList,
                    GameAssetManager.UNITS_FILTER
                    ))
                {
                    foreach(DistanceHit distanceHit in distanceHitsList)
                    {
                        // Get the Unit entity that we detected within the search range
                        Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                        // Check to see if the detected unit is a member of our targetted faction.
                        if(targetUnit.Faction == findTarget.ValueRO.TargetFaction)
                        {
                            // Valid Target.
                            target.ValueRW.TargetEntity = distanceHit.Entity;
                            break;
                        }
                    }

                } // End of collisionWorld.OverlapSphere if statement.
            }
        }
    }
}
