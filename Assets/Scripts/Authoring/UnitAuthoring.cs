using System;

using Unity.Entities;

using UnityEngine;

namespace SF.EntitiesModule
{
    public class UnitAuthoring : MonoBehaviour
    {
        public FactionTypes Faction;

        public class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit() 
                { 
                    Faction = authoring.Faction
                });
            }
        }
    }

    public struct Unit : IComponentData
    {
        public FactionTypes Faction;
    }
}
