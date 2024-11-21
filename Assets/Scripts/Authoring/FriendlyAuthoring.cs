using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule
{
    public class FriendlyAuthoring : MonoBehaviour
    {
        public class FreidnlyAuthoringBaker : Baker<FriendlyAuthoring>
        {
            public override void Bake(FriendlyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Friendly());
            }
        }
    }


    public struct Friendly : IComponentData
    {

    }
}
