using Unity.Entities;
using UnityEngine;

namespace SF
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget());
            }
        }
    }

    public struct FindTarget : IComponentData
    { 

    }
}
