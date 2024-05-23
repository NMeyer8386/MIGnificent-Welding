using Eldemarkki.VoxelTerrain.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace Eldemarkki.VoxelTerrain.World.Chunks
{
    /// <summary>
    /// A class for providing chunks to the world
    /// </summary>
    public class ChunkProvider : MonoBehaviour
    {
        public GameObject chunkParent;
        /// <summary>
        /// The world for which to provide chunks for
        /// </summary>
        public VoxelWorld VoxelWorld { get; set; }

        /// <summary>
        /// Try and do the shift on load
        /// </summary>
        public Transform startingSpot;

        /// <summary>
        /// Instantiates a chunk to <paramref name="chunkCoordinate"/> and initializes it, but does not generate its mesh
        /// </summary>
        /// <param name="chunkCoordinate">The chunk's coordinate</param>
        /// <returns>The new chunk</returns>
        protected ChunkProperties CreateUnloadedChunkToCoordinate(int3 chunkCoordinate)
        {
            Vector3 tempspot = startingSpot.position * .01f;
            int3 worldPosition = chunkCoordinate * VoxelWorld.WorldSettings.ChunkSize;
            GameObject chunkGameObject = Instantiate(VoxelWorld.WorldSettings.ChunkPrefab, worldPosition.ToVectorInt(), Quaternion.identity, chunkParent.transform);

            ChunkProperties chunkProperties = new ChunkProperties
            {
                ChunkGameObject = chunkGameObject,
                MeshCollider = chunkGameObject.GetComponent<MeshCollider>(),
                MeshFilter = chunkGameObject.GetComponent<MeshFilter>(),
                MeshRenderer = chunkGameObject.GetComponent<MeshRenderer>()
            };

            chunkProperties.Initialize(chunkCoordinate, VoxelWorld.WorldSettings.ChunkSize);

            VoxelWorld.ChunkStore.AddChunk(chunkCoordinate, chunkProperties);

            chunkGameObject.transform.position *= 0.01f;

            Vector3 offset = startingSpot.position;

            // Calculate the new position by adding offset to the current position
            Vector3 newPosition = chunkGameObject.transform.position + offset;

            // Move the object to the new position
            chunkGameObject.transform.position = newPosition;


            return chunkProperties;
        }

        /// <summary>
        /// Instantiates a chunk to <paramref name="chunkCoordinate"/>, initializes it and generates its mesh
        /// </summary>
        /// <param name="chunkCoordinate">The coordinate of the chunk to create</param>
        /// <returns>The new chunk</returns>
        public ChunkProperties CreateLoadedChunkToCoordinateImmediate(int3 chunkCoordinate)
        {
            ChunkProperties chunkProperties = CreateUnloadedChunkToCoordinate(chunkCoordinate);
            VoxelWorld.ChunkUpdater.GenerateVoxelDataAndMeshImmediate(chunkProperties);
            return chunkProperties;
        }
    }
}