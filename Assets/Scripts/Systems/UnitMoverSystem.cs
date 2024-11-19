using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace SF.EntitiesModule
{
    partial struct UnitMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create the job for scheduling
            UnitMoverJob unitMoverJob = new UnitMoverJob
            {
                deltaTime = SystemAPI.Time.DeltaTime
            };

            unitMoverJob.ScheduleParallel();

            /*  Pre jobification version of code kept for notes and knowledge.

                // IMPORTANT READ. Notice below we are using a RefRW<IComponent> not just declaring the IComponentData.
                // Remember IComponentData is a struct. We can't just changed it's value because technically a new copy of the struct is made. So we use RefRW to make an editable reference of the struct data.

                // The RW stands for read/write. There is one for RO which is ReadOnly.

                // Local transforms is the entity version of the gameobject transform component when converted to entities.
                // SystemAPI.Query<LocalTransform>() returns an IEnuerator allowing for looping in for each loops.         
                foreach((RefRW<LocalTransform> localTransform, RefRO<UnitMover> UnitMover, RefRW<PhysicsVelocity> physicsVelocity)
                    in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
                {
                    float3 moveDirection = UnitMover.ValueRO.TargetPosition - localTransform.ValueRO.Position;
                    moveDirection = math.normalize(moveDirection);

                    localTransform.ValueRW.Rotation = 
                        math.slerp(localTransform.ValueRO.Rotation,
                                quaternion.LookRotation(moveDirection, math.up()),
                                SystemAPI.Time.DeltaTime * UnitMover.ValueRO.RotationSpeed);

                    physicsVelocity.ValueRW.Linear = moveDirection * UnitMover.ValueRO.MoveSpeed;
                    physicsVelocity.ValueRW.Angular = float3.zero;
                }
             */
        }
    }

    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {

        public float deltaTime;

        // using ref is equivlent to using RefRW
        // using in is equivlent to using RefRO
        public void Execute(
            ref LocalTransform localTransform, 
            in UnitMover unitMove, 
            ref PhysicsVelocity physicsVelocity)
        {
            float3 moveDirection = unitMove.TargetPosition - localTransform.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.Rotation =
                math.slerp(localTransform.Rotation,
                        quaternion.LookRotation(moveDirection, math.up()),
                        deltaTime * unitMove.RotationSpeed);

            physicsVelocity.Linear = moveDirection * unitMove.MoveSpeed;
            physicsVelocity.Angular = float3.zero;
        }
    }
}