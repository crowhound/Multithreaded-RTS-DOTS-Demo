using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule.Combat
{
    public class HealthAuthoring : MonoBehaviour
    {
        public int HealthAmount = 4;

        public class HealthAuthoringBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health()
                {
                    HealthAmount = authoring.HealthAmount
                });
            }
        }
    }

    public struct Health : IComponentData
    {
        public int HealthAmount;
    }
}
