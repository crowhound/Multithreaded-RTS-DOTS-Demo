using System;

using SF.EntitiesModule;

using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

using UnityEngine;

namespace SF.EntitiesModule
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance {  get; private set; }

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;

        public int UnitsPhysicsLayerIndex = 6;

        private Vector2 _selectionStartMousePosition;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {

            if(Input.GetMouseButtonDown(0))
            {
                // Get the screen coordinate of the mouse position.
                _selectionStartMousePosition = Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this,EventArgs.Empty);
            }


            if(Input.GetMouseButtonUp(0))
            {
                // Get the screen coordinate of the mouse position.

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                // Make a query to get the entity units in a system.
                // We set the allocator to temp so we don't keep the allocation in memory.

                // Get a query of all Units already selected so we can cycle and deselect them.
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                                .WithAll<Selected>()
                                                .Build(entityManager);

                // Get the native array of entities from the query
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                // Deselect all units that were previously selected.
                for(int i = 0; i < entityArray.Length; i++)
                {                             
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                }


                Rect selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                float multipleSelectionSizeMin = 40f;
                bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

                if(isMultipleSelection) // Multiple Selection
                {
                    // First we get all Entities with a LocalTransform and Unit IComponentData struct attached.
                    // Than we make sure we only select Units that are selectable. 
                    // Note WithPresent method makes sure they have that IComponentData struct, but it will allow for disabled and enabled Selected IComponentData. 
                    entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                                    .WithAll<LocalTransform, Unit>()
                                                    .WithPresent<Selected>()
                                                    .Build(entityManager);

                    entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                    // Get the native array of our units LocalTransform IDataComponents.
                    NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                    // Remember IComponentData are normally structs so
                    // You can't just modify the value. You have to copy back the new data into the entity array.
                    for(int i = 0; i < localTransformArray.Length; i++)
                    {
                        // Get a copy of the structs current value
                        LocalTransform unitLocalTransform = localTransformArray[i];

                        // Get the screen position of the unit being 
                        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);

                        // Does our selection area rect contain the unit
                        if(selectionAreaRect.Contains(unitScreenPosition))
                        {
                            // Unit is inside the selected Unit area.

                            // Set the units in the selection as selected.
                            entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        }
                    }

                }
                else // Single Selection
                {
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                    UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastInput rayCastInput = new RaycastInput {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter { 
                            BelongsTo = ~0u,
                            CollidesWith = 1u << UnitsPhysicsLayerIndex,
                            GroupIndex = 0,
                        }
                    };

                    if(collisionWorld.CastRay(rayCastInput, out Unity.Physics.RaycastHit raycastHit))
                    {
                        if(entityManager.HasComponent<Unit>(raycastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity,true);
                        }
                    }
                }

                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }

            if(Input.GetMouseButtonDown(1))
            {
                // Get current mouse positon
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();


                EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;

                // Make a query to get the entity units in a system.
                // We set the allocator to temp so we don't keep the allocation in memory.
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                                .WithAll<UnitMover, Selected>()
                                                .Build(entityManager);

                // Get the native array of entities from the query
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                // Get the native array of our Unit Mover IDataComponents.
                NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

                // Setting all Unit movers target position.
                // Remember IComponentData are normally structs so
                // You can't just modify the value. You have to copy back the new data into the entity array.
                for(int i = 0; i < unitMoverArray.Length; i++)
                {
                    // Get a copy of the structs current value
                    UnitMover unitMover = unitMoverArray[i];
                    // Set the structs target position value.
                    unitMover.TargetPosition = mouseWorldPosition;

                    // Set the component data back.
                    // You will need to update the actual array component data still.
                    unitMoverArray[i] = unitMover;
                }

                // Updates the Entity Native Array in the entity query.
                entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(_selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Min(_selectionStartMousePosition.y, selectionEndMousePosition.y)
            );

            Vector2 upperRightCorner = new Vector2(
               Mathf.Max(_selectionStartMousePosition.x, selectionEndMousePosition.x),
               Mathf.Max(_selectionStartMousePosition.y, selectionEndMousePosition.y)
           );
            return new Rect(
                lowerLeftCorner.x,
                lowerLeftCorner.y,
                upperRightCorner.x - lowerLeftCorner.x,
                upperRightCorner.y - lowerLeftCorner.y
            );
        }
    }

}
