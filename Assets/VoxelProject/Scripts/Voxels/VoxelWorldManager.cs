using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelWorldManager : MonoBehaviour
{
    [HideInInspector]
    // Define the container that will handle the render meshes for a given collection of voxels
    // Meshes is intended to be plural as this container could contain various meshes at various levels of detail.
    public VoxelContainer voxelMeshContainer;

    // Vector3Int's are used as they use up less memory and also
    // implement the IEquatable interface making searching the Dictionary nice and fast
    public Dictionary<Vector3Int, Chunk> voxelSourceDataDictionary;

    //Use streaming assets for the file path. e.g. "blue.txt" "voxtest.txt"
    private string voxelDataFilePath = Path.Combine(Application.streamingAssetsPath, "voxtest.txt");

    [Header("Voxel Camera, drag in the Main Camera")]
    public Camera mainCamera;

    private bool filesLoaded = false;
    [HideInInspector]
    public bool quitting = false;
    private bool voxelsReady = false;

    [HideInInspector]
    public int tempwidthX = 0;
    [HideInInspector]
    public int tempheightY = 0;
    [HideInInspector]
    public int tempdepthZ = 0;

    public static VoxelWorldSettings WorldSettings;
    private static VoxelWorldManager _instance;

    [Header("Voxel World Attributes")]
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public VoxelWorldSettings worldSettings;

    [Header("Voxel Configuration Attributes")]
    public int voxelChunkSize = 16;
    public int voxelChunkFieldOfViewMultiplier = 1;
    public string voxelMeshContainerTagName;

    public static VoxelWorldManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<VoxelWorldManager>();
            return _instance;
        }
    }

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

        try
        {
            SourceDataTextFileLoaderAsDictionary loader = new SourceDataTextFileLoaderAsDictionary(voxelChunkSize);
            voxelSourceDataDictionary = loader.LoadSourceData(voxelDataFilePath);
            tempwidthX = loader.widthX;
            tempheightY = loader.heightY;
            tempdepthZ = loader.depthZ;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataTextFileLoader.widthX;
        WorldSettings.maxHeightY = SourceDataTextFileLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataTextFileLoader.depthZ;

        //Initialize the voxel containers
        InitialiseContainers(voxelMeshContainerTagName);

        Debug.Log("Voxels ready check starting...");
        while (voxelSourceDataDictionary == null && voxelSourceDataDictionary.Count <= 0)
        {
            Debug.Log("Voxels not yet ready...");
            voxelsReady = false;
        }
        Debug.Log("Voxels now ready...");
        voxelsReady = true;
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        quitting = true;
    }

    private void Update()
    {
        // Don't attempt any update loop if unity is either quitting or the voxel load isn't complete, it's just not worth it!
        if (quitting == false && voxelsReady == true) {
            voxelMeshContainer.ClearData();
            VoxelComputeManager.Instance.GenerateVoxelData(ref voxelMeshContainer, ref mainCamera, true);
        }
    }    

    void InitialiseContainers(string meshContainerName = "OuterContainerDic")
    {
        // Initialise an individual Voxel mesh container
        GameObject meshContainer = InstantiateContainerPosition(meshContainerName, Vector3Int.zero);

        // Tag the outer container for collision detection purposes
        meshContainer.tag = meshContainerName;
        
        // Add a collider
        SetCollider(meshContainer);

        // Attach the new meshContainer to the main scene's mesh container
        voxelMeshContainer = meshContainer.AddComponent<VoxelContainer>();
        voxelMeshContainer.Initialize(worldMaterial, Vector3Int.zero);

        VoxelComputeManager.Instance.Initialize(1);
    }

    GameObject InstantiateContainerPosition(string name, Vector3Int position)
    {
        GameObject cont = new GameObject(name);
        cont.transform.parent = transform;
        cont.transform.position = position;
        cont.transform.Rotate(270, 0, 0); // Adjust this as necessary to correct the orientation
        return cont;
    }

    void SetCollider(GameObject container)
    {
        // Add a collider to the container for collision detection
        BoxCollider collider = container.AddComponent<BoxCollider>();
        collider.isTrigger = true; // Enable IsTrigger
        collider.size = new Vector3Int(11, 9, 9); // Adjust the size as needed
    }
}