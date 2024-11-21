using System;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

                // Deselect all units that were previously selected.
                for(int i = 0; i < entityArray.Length; i++)
                {                             
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                    
                    // Raise the deselection event for the units that were previously selected, but are not not selected.
                    Selected selected = selectedArray[i];
                    selected.OnDeselected = true;

                    // We can't just set the array because above where we disable the selected entity components it will change the result of the saved entity
                    // selectedArray[i] = selected; This will error out due to the above because now
                    // selectedArray.length = 0

                    // This is an example situation where we use entity manager.setComponent 
                    // instead of writing back values to an entity queru array.
                    entityManager.SetComponentData(entityArray[i],selected);
                }


                Rect selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                float multipleSelectionSizeMin = 50f;
                bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

                if(isMultipleSelection) // Multiple Selection
                {
                    /* First we get all Entities with a LocalTransform and Unit IComponentData struct attached.
                    * Than we make sure we only select Units that are selectable. 
                     Note WithPresent method makes sure they have that IComponentData struct, but it will allow for disabled and enabled Selected IComponentData. 
                    */

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

                            Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                            selected.OnSelected = true;
                            entityManager.SetComponentData(entityArray[i], selected);
                        }
                    }

                }
                else // Single Selection
                {
                    /* This Entity query syntax does the exact samething 
                     * as the entityManager.CreateEntityQuery. One just has a LINQ like format
                    new EntityQueryBuilder(Allocator.Temp)
                                                    .WithAll<PhysicsWorldSingleton>()
                                                    .Build(entityManager);
                    */
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

                    // PhysicsWorldSingleton is used to access Entity world simulations for physics.
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    // CollisionWorld is a collection of rigidbodies wrapped by a bounding volume hierarchy
                    // To allow for collisioon queries like raycast, overlap testing, and so forth.

                    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                    UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastInput rayCastInput = new RaycastInput {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter { 
                            BelongsTo = ~0u, // u for setting a uint. ~ inverts the zero values in the bitmask to all ones.
                            // 1 in a layer bitmask is physics layer index 0. 
                            // We use a bit shift operator to move the value over to the index we want.
                            // Physics layer on layer index 6 is 1u << 6 as a uint.
                            CollidesWith = 1u << GameAssetManager.UNITS_LAYER,
                            GroupIndex = 0,
                        }
                    };

                    if(collisionWorld.CastRay(rayCastInput, out Unity.Physics.RaycastHit raycastHit))
                    {
                        if(entityManager.HasComponent<Unit>(raycastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity,true);
                            
                            // Raise the selection event on the entity.
                            Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                            selected.OnSelected = true;
                            entityManager.SetComponentData(raycastHit.Entity,selected);
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

                NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition,entityArray.Length);

                // Setting all Unit movers target position.
                // Remember IComponentData are normally structs so
                // You can't just modify the value. You have to copy back the new data into the entity array.
                for(int i = 0; i < unitMoverArray.Length; i++)
                {
                    // Get a copy of the structs current value
                    UnitMover unitMover = unitMoverArray[i];
                    // Set the structs target position value.
                    unitMover.TargetPosition = movePositionArray[i];

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

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount,Allocator.Temp);

            if(positionCount == 0)
            {
                return positionArray;
            }
            positionArray[0] = targetPosition;
            if(positionCount == 1)
            {
                return positionArray;
            }

            float ringSize = 2.2f;
            int ring = 0;
            int positionIndex = 1;

            
            while(positionIndex < positionCount)
            {
                // On the inermost position ring have only three positions for the units
                int ringPositionCount = 3 + ring * 2;

                for(int i = 0; i < ringPositionCount; i++ )
                {
                    float angle = i * (math.PI2 / ringPositionCount);
                    // We start with a vector facing directly to the right.
                    // Than multiply the angle to make the ring positions.
                    float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1),0,0));
                    float3 ringPosition = targetPosition + ringVector;

                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    // Have we generated all the positions for the ring
                    if(positionIndex >= positionCount)
                    {
                        break;
                    }
                }

                ring++;

            } // End of while Loop

            return positionArray;
        }
    }
}
