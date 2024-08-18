using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Vector3Int's are used as they use up less memory than floats and also

    [Header("Voxel Mesh Settings")]
    [SerializeField]
    public VoxelMeshConfigurationSettings voxelMeshConfigurationSettings;

    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public WorldSettings worldSettings;    
    //public VoxelCell[,,] voxelDictionary;
    public Dictionary<Vector3Int,Voxel> voxelDictionary;

    // The based dictionary to store chunks by vector location
    public Dictionary<Vector3Int, Chunk> voxelChunks;

    // D AS L public Dictionary<long,VoxelCell> voxelDictionary;
    public int voxelsSelected = 0;

    //[HideInInspector]
    // CULLING public VoxelGrid voxelGrid = null;

    public MenuHandler menuHandler;

    void Start()
    {
/*        if (animator == null)
            animator = GameObject.Find("Transition").GetComponent<Animator>();*/

        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        WorldSettings = worldSettings;
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
            // Calculate the chunk's starting worldPosition based on the global worldPosition
            Vector3Int chunkPosition = new Vector3Int(
                Mathf.FloorToInt(globalPosition.x / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize,
                Mathf.FloorToInt(globalPosition.y / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize,
                Mathf.FloorToInt(globalPosition.z / WorldManager.Instance.worldSettings.chunkSize) * WorldManager.Instance.worldSettings.chunkSize
            );

            // Retrieve and return the chunk at the calculated worldPosition
            if (chunks.TryGetValue(chunkPosition, out Chunk chunk))
            {
                return chunk;
            }

            // Return null if no chunk exists at the worldPosition
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
    public float nearClippingDistance = 85f;
    public bool autoRefresh = false;
}