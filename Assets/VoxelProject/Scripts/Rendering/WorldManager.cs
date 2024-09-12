using System.Collections.Generic;
using UnityEngine;

//Settings for the world
public class WorldManager : MonoBehaviour
{
    // Vector3Int's are used as they use up less memory than floats and also
    [Header("Voxel Mesh Settings")]
    [SerializeField]
    public VoxelMeshConfigurationSettings voxelMeshConfigurationSettings;

    public Material worldMaterial;
    public WorldSettings worldSettings;

    // The Vector3Int based dictionary to store Voxels by vector location
    public Dictionary<Vector3Int,Voxel> voxelDictionary;

    // The Vector3Int based dictionary to store chunks by vector location
    public Dictionary<Vector3Int, Chunk> voxelChunks;

    public int voxelsSelected = 0;
    public int chunksOnDisplay = 0;

    public MenuHandler menuHandler;

    public static WorldSettings WorldSettings;
    private static WorldManager _instance;

    //In the Start we implement a Singleton pattern to ensure that only one instance of the WorldManager is created
    void Start()
    {
        //Ensures only one instance of the WolrdManager is active
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

    public static WorldManager Instance
    {
        get
        {   //If the instance is null, we find the first object of type WorldManager
            if (_instance == null)
                _instance = FindFirstObjectByType<WorldManager>();
            return _instance;
        }
    }
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
    public bool grayScaleMode = true;
    public bool sparseVoxels = false;
    public Vector3Int intialCameraPosition = Vector3Int.zero;
}