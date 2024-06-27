using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public WorldSettings worldSettings;
    // oldway public VoxelCell[] sourceData;
    public Voxel[,,] sourceData;

    public Container container;

    void Start()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
        }

        sourceData = SourceDataLoader.LoadSourceData();
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataLoader.widthX;
        WorldSettings.maxHeightY = SourceDataLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataLoader.depthZ;

        ComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(worldMaterial, Vector3.zero);

        ComputeManager.Instance.GenerateVoxelData(ref container);
    }

    public static WorldSettings WorldSettings;
    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<WorldManager>();
            return _instance;
        }
    }

/*    public Chunk GetChunkAt(Vector3 globalPosition)
    {
        // Calculate the chunk's starting position based on the global position
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize,
            Mathf.FloorToInt(globalPosition.y / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize,
            Mathf.FloorToInt(globalPosition.z / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize
        );

        // Retrieve and return the chunk at the calculated position
        if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))
        {
            return chunk;
        }

        // Return null if no chunk exists at the position
        return null;
    }*/
}


[System.Serializable]
public class WorldSettings
{
    public int containerSize = 16;
    public int maxHeight = 128;
    public int maxWidthX = 0;
    public int maxDepthZ = 0;
    public int maxHeightY = 0;
    public int chunkSize = 32;
}