using Unity.Entities;
using UnityEngine;

namespace SF
{
    public class ZombieAuthoring : MonoBehaviour
    {

        public class ZombieAuthoringBaker : Baker<ZombieAuthoring>
        {
            public override void Bake(ZombieAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Zombie());
            }
        }
    }

    public struct Zombie : IComponentData
    {

    }
}
