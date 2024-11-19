using Unity.Entities;
using UnityEngine;

namespace SF.EntitiesModule
{
    public class SelectedAuthoring : MonoBehaviour
    {
        // The selected circle game object.
        public GameObject VisualGameObject;
        public float ShowScale = 2;

        class SelectedAuthoringBaker : Baker<SelectedAuthoring>
        {
            public override void Bake(SelectedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selected {
                    VisualEntity = GetEntity(authoring.VisualGameObject, TransformUsageFlags.Dynamic),
                    ShowScale = authoring.ShowScale
                });
                // Set the entities to not beselected by default.
                SetComponentEnabled<Selected>(entity, false);
            }
        }
    }

    public struct Selected : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// This is the entity that represents the game object with the selection circle mesh on it.
        /// </summary>
        public Entity VisualEntity;
        public float ShowScale;
    }
}
