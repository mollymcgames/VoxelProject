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
    public Dictionary<Vector3Int,Voxel> voxelDictionary;

    // The based dictionary to store chunks by vector location
    public Dictionary<Vector3Int, Chunk> voxelChunks;

    public int voxelsSelected = 0;

    public MenuHandler menuHandler;

    public static WorldSettings WorldSettings;
    private static WorldManager _instance;

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

    public static WorldManager Instance
    {
        get
        {
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
}