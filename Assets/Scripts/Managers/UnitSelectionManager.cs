using System;

using SF.EntitiesModule;

using Unity.Collections;
using Unity.Entities;

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
            }


            if(Input.GetMouseButtonUp(0))
            {
                // Get the screen coordinate of the mouse position.
                Vector2 selectionStartMousePosition = Input.mousePosition;
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
    }
}
