using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
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

    //[Header("Voxel Camera, drag in the Main Camera")]
    Camera mainCamera;

    //private bool filesLoaded = false;

    [HideInInspector]
    public bool quitting = false;
    private bool voxelsReady = false;

    public static VoxelWorldSettings WorldSettings;
    private static VoxelWorldManager _instance;

    [Header("Voxel World Attributes")]
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public VoxelWorldSettings worldSettings;

    [Header("Voxel Mesh Settings")]
    [SerializeField]
    public VoxelMeshConfigurationSettings voxelMeshConfigurationSettings;

    public int offsetXBackToZero = 0;
    public int offsetYBackToZero = 0;
    public int offsetZBackToZero = 0;

    [HideInInspector]
    public bool doSceneSwitch = false;

    [HideInInspector]
    public string voxelFileFormat = "nii";

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
            //DontDestroyOnLoad(this);
        }       

        try
        {
            // Load in one voxel model, currently support types are .txt and .nii files.
            // If more types need supporting, will need additional SourceData loader implementations.
            // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
            ASourceDataLoader loader = DataLoaderUtils.LoadDataFile();
            voxelSourceDataDictionary = loader.LoadSourceData(voxelMeshConfigurationSettings.voxelDataFilePath);
            voxelFileFormat = DataLoaderUtils.GetDataFileFormat();

            WorldSettings = worldSettings;
            WorldSettings.maxWidthX = loader.widthX;
            WorldSettings.maxHeightY = loader.heightY;
            WorldSettings.maxDepthZ = loader.depthZ;
            calculateOffsetsBackToZero(loader);
            mainCamera = Camera.main;
            mainCamera.transform.position = CalculateMiddlePoint((float)loader.minX, (float)loader.maxX, (float)loader.minY, (float)loader.maxY, (float)loader.minZ, (float)loader.maxZ);
            Debug.Log("Camera pointed at: " + mainCamera.transform.position);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        Debug.Log("World x:" + WorldSettings.maxWidthX);
        Debug.Log("World y:" + WorldSettings.maxHeightY);
        Debug.Log("World z:" + WorldSettings.maxDepthZ);

        Debug.Log("Voxels ready check starting...");
        while (voxelSourceDataDictionary == null && voxelSourceDataDictionary.Count <= 0)
        {
            Debug.Log("Voxels not yet ready...");
            voxelsReady = false;
        }
        Debug.Log("Voxels now ready...");
        voxelsReady = true;

        //Initialise the voxel containers
        InitialiseContainers(voxelMeshConfigurationSettings.voxelMeshContainerTagName);
    }

    private Vector3 CalculateMiddlePoint(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
    {
        float xMid = (xMin + xMax) / 2f;
        float yMid = (yMin + yMax) / 2f;
        float zMid = (zMin + zMax) / 2f;

        return new Vector3(xMid, yMid, zMid);
    }

    private void calculateOffsetsBackToZero(ASourceDataLoader loader)
    {
        offsetXBackToZero = -loader.minX;
        offsetYBackToZero = -loader.minY;
        offsetZBackToZero = -loader.minZ;
        Debug.Log("World offset x:" + offsetXBackToZero);
        Debug.Log("World offset y:" + offsetYBackToZero);
        Debug.Log("World offset z:" + offsetZBackToZero);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        quitting = true;
    }

    bool firstLook = true;
    private void Update()
    {
        // Don't attempt any update loop if unity is either quitting or the voxel load isn't complete, it's just not worth it!
        if (quitting == false && voxelsReady == true && VoxelWorldManager.Instance.doSceneSwitch == false) {
            //voxelMeshContainer.ClearData();           
            VoxelWorldManager.Instance.voxelMeshContainer.RenderMesh(GeneralMethods.AdjustMaterialTransparency(ref voxelMeshContainer));

            // noNoise? VoxelComputeManager.Instance.GenerateVoxelData(ref voxelMeshContainer);
            if (firstLook)
            {
                Debug.Log("Looking at..." + voxelMeshContainer.containerPosition);
                mainCamera.transform.LookAt(voxelMeshContainer.containerPosition);
                firstLook = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected: " + other.tag + " - " + other.name);
        if (other.CompareTag("MainCamera"))
        {
            // Toggle the voxel data generation
/*            isGeneratingOuter = !isGeneratingOuter;
            container.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref container, isGeneratingOuter);
*/        }
    }

    void InitialiseContainers(string meshContainerName)
    {
        // Initialise an individual Voxel mesh container
        GameObject meshContainer = InstantiateContainerPosition(meshContainerName, Vector3Int.zero);

        // Tag the outer container for collision detection purposes
        meshContainer.tag = meshContainerName;
        
        // Add a collider
        SetCollider(meshContainer);

        // Attach the new meshContainer to the main scene's mesh container
        voxelMeshContainer = meshContainer.AddComponent<VoxelContainer>();
        voxelMeshContainer.Initialise(worldMaterial, Vector3Int.zero);

        // noNoise? VoxelComputeManager.Instance.Initialise(1);
    }

    GameObject InstantiateContainerPosition(string name, Vector3Int position)
    {
        GameObject cont = new GameObject(name);
        cont.transform.parent = transform;
        cont.transform.position = position;
        // Adjust this as necessary to correct the orientation
        cont.transform.Rotate(270, 0, 0); 
        return cont;
    }

    void SetCollider(GameObject container)
    {
        // Add a collider to the container for collision detection
        BoxCollider collider = container.AddComponent<BoxCollider>();
        // Enable IsTrigger
        collider.isTrigger = true;
        // Adjust the size as needed
        collider.size = new Vector3Int(21, 18, 7);
        collider.center = 
              new Vector3Int(VoxelWorldManager.Instance.offsetXBackToZero, VoxelWorldManager.Instance.offsetYBackToZero, VoxelWorldManager.Instance.offsetZBackToZero)
            + new Vector3Int(VoxelWorldManager.Instance.voxelMeshConfigurationSettings.domainOffsetX, VoxelWorldManager.Instance.voxelMeshConfigurationSettings.domainOffsetY, VoxelWorldManager.Instance.voxelMeshConfigurationSettings.domainOffsetZ);     
    }
}