using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        /// <summary>
        /// How many second before trying to shoot a target.
        /// </summary>
        public float TimerMax;

        public class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack() { TimerMax = authoring.TimerMax });

            }
        }
    }

    public struct ShootAttack : IComponentData
    {
        public float Timer;
        /// <summary>
        /// How many second before trying to shoot a target.
        /// </summary>
        public float TimerMax;
    }
}
