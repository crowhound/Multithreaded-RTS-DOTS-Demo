using Unity.Entities;

using UnityEngine;

namespace SF.EntitiesModule
{
    public class TargetAuthoring : MonoBehaviour
    {
        public class TargetAuthoringBaker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Target());
            }
        }

    }


    public struct Target : IComponentData
    {
        public Entity TargetEntity;
    }
}
