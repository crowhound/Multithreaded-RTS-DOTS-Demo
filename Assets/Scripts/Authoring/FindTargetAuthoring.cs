using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public float SearchRange;

        public class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget() { SearchRange = authoring.SearchRange});
            }
        }
    }

    public struct FindTarget : IComponentData
    {
        public float SearchRange;
    }
}
