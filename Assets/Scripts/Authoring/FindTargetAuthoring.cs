using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public float SearchRange;
        public FactionTypes TargetFaction;
        public float TimerMax;

        public class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget() 
                {
                    SearchRange = authoring.SearchRange,
                    TargetFaction = authoring.TargetFaction,
                    TimerMax = authoring.TimerMax,
                });
            }
        }
    }

    public struct FindTarget : IComponentData
    {
        public float SearchRange;
        public FactionTypes TargetFaction;
        public float Timer;
        public float TimerMax;
    }
}
