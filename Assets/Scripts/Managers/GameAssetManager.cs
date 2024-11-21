using Unity.Physics;

using UnityEngine;

namespace SF
{
    public class GameAssetManager : MonoBehaviour
    {
        /// <summary>
        /// The physics layer for the Units.
        /// </summary>
        public const int UNITS_LAYER = 6;

        /// <summary>
        /// The Units physics collision filter for quickly querying a filtered set of collision based on the
        /// <see cref="UNITS_LAYER"/> physics layer.
        /// </summary>
        public static readonly CollisionFilter UNITS_FILTER = new CollisionFilter
        {
            // u for setting a uint. ~ inverts the zero values in the bitmask to all ones.
            // 1 in a layer bitmask is physics layer index 0. 
            // We use a bit shift operator to move the value over to the index we want.
            // Physics layer on layer index 6 is 1u << 6 as a uint.

            BelongsTo = ~0u,
            CollidesWith = 1u << UNITS_LAYER,
            GroupIndex = 0,
        };

    public GameAssetManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
