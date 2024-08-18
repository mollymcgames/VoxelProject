using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class OuterWorldManager : MonoBehaviour
{
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public OuterWorldSettings worldSettings;
    public Dictionary<Vector3Int, Voxel> sourceDataInner;
    public Dictionary<Vector3Int, Voxel> sourceDataOuter;

    public OuterContainer containerOuter;
    public OuterContainer containerInner;
    public OuterContainer containerOuterDic;
    public OuterContainer containerInnerDic;

    // Vector3Int's are used as they use up less memory and also
    // implement the IEquatable interface making searching the Dictionary nice and fast
    public Dictionary<Vector3Int, Chunk> sourceDataInnerDictionary;
    public Dictionary<Vector3Int, Chunk> sourceDataOuterDictionary;

    //Use streaming assets for the file path
    private string filepathInner = Path.Combine(Application.streamingAssetsPath, "blue.txt");
    private string filepathOuter = Path.Combine(Application.streamingAssetsPath, "voxtest.txt");

    public Camera mainCamera;

    private bool filesLoaded = false;
    public bool quitting = false;
    private bool voxelsReady = false;

    public int tempwidthX = 0;
    public int tempheightY = 0;
    public int tempdepthZ = 0;

    public int chunkOuterSize = 16;
    public int chunkInnerSize = 2;

    public int chunkFieldOfViewMultiplierInner = 1;
    public int chunkFieldOfViewMultiplierOuter = 1;


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

        SourceDataTextFileLoader innerLoader = new SourceDataTextFileLoader(0);
        SourceDataTextFileLoader outerLoader = new SourceDataTextFileLoader(0);

        sourceDataInner = innerLoader.LoadSourceData(filepathInner);
        sourceDataOuter = outerLoader.LoadSourceData(filepathOuter);

        SourceDataTextFileLoaderAsDictionary loaderInnerAsDict = null;
        try
        {
            loaderInnerAsDict = new SourceDataTextFileLoaderAsDictionary(chunkInnerSize);
            sourceDataInnerDictionary = loaderInnerAsDict.LoadSourceData(filepathInner);
            tempwidthX = loaderInnerAsDict.X;
            tempheightY = loaderInnerAsDict.Y;
            tempdepthZ = loaderInnerAsDict.Z;

            sourceDataOuterDictionary = loaderInnerAsDict.LoadSourceData(filepathOuter);

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        SourceDataTextFileLoaderAsDictionary loaderOuterAsDict = null;
        try
        {
            loaderOuterAsDict = new SourceDataTextFileLoaderAsDictionary(chunkOuterSize);
            sourceDataOuterDictionary = loaderOuterAsDict.LoadSourceData(filepathOuter);
            tempwidthX = loaderOuterAsDict.X;
            tempheightY = loaderOuterAsDict.Y;
            tempdepthZ = loaderOuterAsDict.Z;                       
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        WorldSettings = worldSettings;
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = outerLoader.X;
        WorldSettings.maxHeightY = outerLoader.Y;
        WorldSettings.maxDepthZ = outerLoader.Z;

        //Initialise the outer containers
        InitialiseContainers();

        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Voxels ready check starting...");
        while (sourceDataInner == null &&
            sourceDataOuter == null &&
            sourceDataInner.Count <= 0 &&
            sourceDataOuter.Count <= 0 &&
            sourceDataInnerDictionary == null &&
            sourceDataInnerDictionary.Count <= 0 &&
            sourceDataOuterDictionary == null &&
            sourceDataOuterDictionary.Count <= 0)
        {
            // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Voxels not yet ready...");
            voxelsReady = false;
        }
        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Voxels now ready...");
        voxelsReady = true;
    }

    private void Update()
    {
        if (quitting == false && voxelsReady == true) {
            containerOuter.ClearData();
            containerInner.ClearData();
            containerOuterDic.ClearData();
            containerInnerDic.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref containerOuter, ref mainCamera, true);
        }
    }    

    void InitialiseContainers()
    {
        //The firsts container initilised
        GameObject contInner = InstantiateContainerPosition("InnerContainer", Vector3Int.zero);
        contInner.tag = "InnerContainer"; // Tag the inner container for collision detection
        containerInner = contInner.AddComponent<OuterContainer>();
        containerInner.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contInner);

        //The second container initialised
        GameObject contOuter = InstantiateContainerPosition("OuterContainer", Vector3Int.zero);
        contOuter.tag = "OuterContainer"; //Tag the outer container for collision detection
        containerOuter = contOuter.AddComponent<OuterContainer>();
        containerOuter.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contOuter);

        //The INNER chunk container initialised
        GameObject contInnerDic = InstantiateContainerPosition("InnerContainerDic", Vector3Int.zero);
        contInnerDic.tag = "InnerContainerDic"; // Tag the inner container for collision detection
        containerInnerDic = contInnerDic.AddComponent<OuterContainer>();
        containerInnerDic.Initialize(worldMaterial, Vector3Int.zero);
        SetCollider(contInnerDic);

        //The OUTER chunk container initialised
        GameObject contOuterDic = InstantiateContainerPosition("OuterContainerDic", Vector3Int.zero);
        contOuterDic.tag = "OuterContainerDic"; // Tag the outer container for collision detection
        containerOuterDic = contOuterDic.AddComponent<OuterContainer>();
        containerOuterDic.Initialize(worldMaterial, Vector3Int.zero);
        SetCollider(contOuterDic);


        OuterComputeManager.Instance.Initialize(1);
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
        // // Add a collider to the container for collision detection
        BoxCollider collider = container.AddComponent<BoxCollider>();
        collider.isTrigger = true; // Enable IsTrigger
        collider.size = new Vector3Int(11, 9, 9); // Adjust the size as needed
    }

    
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        quitting = true;
    }
    
    //check for a collision with the MainCamera to switch between the two voxel meshes

    public static OuterWorldSettings WorldSettings;
    private static OuterWorldManager _instance;

    public static OuterWorldManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<OuterWorldManager>();
            return _instance;
        }
    }
}

[System.Serializable]
public class OuterWorldSettings
{
    public int containerSize = 16;
    public int maxHeight = 128;
    public int maxWidthX = 0;
    public int maxDepthZ = 0;
    public int maxHeightY = 0;
}