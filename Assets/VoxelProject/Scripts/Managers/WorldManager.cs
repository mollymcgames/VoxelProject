using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Vector3Int's are used as they use up less memory and also
    // implement the IEquatable interface making searching the Dictionary nice and fast
    public Dictionary<Vector3Int, Chunk> voxelSourceDataDictionary;

    [Header("Voxel Mesh Settings")]
    [SerializeField]
    public VoxelMeshConfigurationSettings voxelMeshConfigurationSettings;

    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public WorldSettings worldSettings;
    // oldway public VoxelCell[] sourceData;
    public Voxel[,,] sourceData;


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
        //ComputeManager.Instance.GenerateVoxelData(ref container, 0);
    }

    // void Update(){
    //     // key is 0 for now, will be changed to a more appropriate key later
    //     if (Input.GetKeyDown(KeyCode.Alpha0))
    //     {
    //         Debug.Log("0 key pressed, generating voxel data.");
    //         ComputeManager.Instance.GenerateVoxelData(ref container, 0);
    //     } else if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         Debug.Log("1 key pressed, generating voxel data.");
    //         ComputeManager.Instance.GenerateVoxelData(ref container, 1);
    //     }
    // }

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