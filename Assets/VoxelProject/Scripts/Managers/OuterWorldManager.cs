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
    public VoxelCell[] sourceDataInner;
    public VoxelCell[] sourceDataOuter;

    public OuterContainer containerOuter;
    public OuterContainer containerInner;
    public OuterContainer containerInnerDic;

    public Dictionary<Vector3Int, Chunk> sourceDataInnerDictionary;// = new Dictionary<Vector3Int, Chunk>();
    public Dictionary<Vector3Int, Chunk> sourceDataOuterDictionary;// = new Dictionary<Vector3Int, Chunk>();

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

        sourceDataInner = SourceDataTextFileLoader.LoadSourceData(filepathInner);
        sourceDataOuter = SourceDataTextFileLoader.LoadSourceData(filepathOuter);

        try
        {
            SourceDataTextFileLoaderAsDictionary loader = new SourceDataTextFileLoaderAsDictionary();
            sourceDataInnerDictionary = loader.LoadSourceData(filepathInner);
            tempwidthX = loader.widthX;
            tempheightY = loader.heightY;
            tempdepthZ = loader.depthZ;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        //StartCoroutine(LoadData());

        WorldSettings = worldSettings;
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataTextFileLoader.widthX;
        WorldSettings.maxHeightY = SourceDataTextFileLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataTextFileLoader.depthZ;

        //Initialize the outer containers
        InitialiseContainers();

        /*        int breakout = 0;
                //while (filesLoaded == false || breakout < 100 )
                while (breakout < 300)
                {
                    Debug.Log("Waiting for files to load...:"+filesLoaded);
                    breakout++;
                }
                chunksDictionary = ConstructChunks(sourceDataInner);
        */

        Debug.Log("Voxels ready check starting...");
        while (sourceDataInner == null &&
            sourceDataOuter == null &&
            sourceDataInner.Length <= 0 &&
            sourceDataOuter.Length <= 0 &&
            sourceDataInnerDictionary == null &&
            sourceDataInnerDictionary.Count <= 0)
        {
            Debug.Log("Voxels not yet ready...");
            voxelsReady = false;
        }
        Debug.Log("Voxels now ready...");
        voxelsReady = true;
    }

    IEnumerator LoadData()
    {
        filesLoaded = false;
        Debug.Log("Waiting for files to load...");
        yield return new WaitUntil( () => 
            sourceDataInner != null && 
            sourceDataOuter != null &&
            sourceDataInner.Length > 0 &&
            sourceDataOuter.Length > 0 );
        Debug.Log("Files loaded!");
        filesLoaded = true;
    }

    void InitialiseContainers()
    {
        //The firsts container initilised
        GameObject contInner = InstantiateContainerPosition("InnerContainer", Vector3.zero);
        contInner.tag = "InnerContainer"; // Tag the inner container for collision detection
        containerInner = contInner.AddComponent<OuterContainer>();
        containerInner.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contInner);


        //The second container initialised
        GameObject contOuter = InstantiateContainerPosition("OuterContainer", Vector3.zero);
        contOuter.tag = "OuterContainer"; //Tag the outer container for collision detection
        containerOuter = contOuter.AddComponent<OuterContainer>();
        containerOuter.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contOuter);

        //The firsts container initilised
        GameObject contInnerDic = InstantiateContainerPosition("InnerContainerDic", Vector3.zero);
        contInnerDic.tag = "InnerContainerDic"; // Tag the inner container for collision detection
        containerInnerDic = contInnerDic.AddComponent<OuterContainer>();
        containerInnerDic.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contInnerDic);

        OuterComputeManager.Instance.Initialize(1);
    }


    GameObject InstantiateContainerPosition(string name, Vector3 position)
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
        collider.size = new Vector3(11, 9, 9); // Adjust the size as needed
    }

    
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        quitting = true;
    }

    bool isGeneratingOuter = true;
    private void Update()
    {
        if (quitting == false && voxelsReady == true) {
            containerOuter.ClearData();
            containerInner.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref containerOuter, ref mainCamera, true);
        }
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