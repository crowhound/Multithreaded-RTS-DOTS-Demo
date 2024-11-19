using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;


namespace SF.EntitiesModule
{
    public class UnitMoverAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        public float RotationSpeed;

        // This classes bake method is automatically called by Unity
        // when subscenes are baking gameobjects into entities.
        public class Baker : Baker<UnitMoverAuthoring>
        {
            // This bake override method spits out the monobehavior component so we can use it's value 
            // to set the starting value of our baked ECS IComponentData.
            public override void Bake(UnitMoverAuthoring authoring)
            {
                // This gets the entity that is the gameobject baked and converted over to an entity.
                Entity entity =  GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover 
                { 
                    MoveSpeed = authoring.MoveSpeed, 
                    RotationSpeed = authoring.RotationSpeed
                });
            }
        }
    }

    public struct UnitMover : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float3 TargetPosition;
    }
}
