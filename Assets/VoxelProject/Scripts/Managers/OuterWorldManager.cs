using System.Collections;
using System.Collections.Generic;
using System.IO;
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


    //Use streaming assets for the file path
    private string filepathInner = Path.Combine(Application.streamingAssetsPath, "blue.txt");
    // private string filepathInner = "Assets/Resources/blue.txt";
    private string filepathOuter = Path.Combine(Application.streamingAssetsPath, "voxtest.txt");
    // private string filepathOuter = "Assets/Resources/voxtest.txt";

    public Camera mainCamera;

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
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataTextFileLoader.widthX;
        WorldSettings.maxHeightY = SourceDataTextFileLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataTextFileLoader.depthZ;

        //Initialize the outer containers
        InitialiseContainers();

    }

    void InitialiseContainers()
    {

        //The firsts container initilised
        GameObject contInner = InstantiateContainerPosition("InnerContainer", Vector3.zero);
        contInner.tag = "InnerContainer";     // Set the tag for collision detection
        containerOuter = contInner.AddComponent<OuterContainer>();
        containerOuter.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contInner);


        //The second container initialised
        GameObject contOuter = InstantiateContainerPosition("OuterContainer", Vector3.zero);
        contOuter.tag = "OuterContainer";
        containerInner = contOuter.AddComponent<OuterContainer>();
        containerInner.Initialize(worldMaterial, Vector3.zero);
        SetCollider(contOuter);
  
        
        // // Tag the inner container
        // GameObject outerContainer = GameObject.FindWithTag("OuterContainer");
        // if (outerContainer == null)
        // {
        //     Debug.LogError("OuterContainer tag not found. Make sure the inner container is tagged correctly.");
        // }

        OuterComputeManager.Instance.Initialize(1);


        // OuterComputeManager.Instance.GenerateVoxelData(ref container, ref mainCamera, true);

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

    public bool quitting = false;
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        quitting = true;
    }

    bool isGeneratingOuter = true;
    private void Update()
    {
        if (quitting == false) {
            // if (Input.GetKeyDown(KeyCode.Z))
            // {
            //     isGeneratingOuter = !isGeneratingOuter;
            //     container.ClearData();
            //     OuterComputeManager.Instance.GenerateVoxelData(ref container, ref mainCamera, isGeneratingOuter);
            // }

            //Debug.Log("Camera position: " + mainCamera.transform.position);
            //if the camera is a certain distance away from the container, switch to the other container
            // if (!isGeneratingOuter && Vector3.Distance(mainCamera.transform.position, container.transform.position) > 18)
            // {
                // isGeneratingOuter = !isGeneratingOuter;
                containerOuter.ClearData();
                containerInner.ClearData();
                OuterComputeManager.Instance.GenerateVoxelData(ref containerOuter, ref mainCamera, true);
                // OuterComputeManager.Instance.GenerateVoxelData(ref containerOuter, ref mainCamera, false);
            // }
            // else
            // {
            //     OuterComputeManager.Instance.GenerateVoxelData(ref container, ref mainCamera, isGeneratingOuter);
            // }
        }
    }
    
    //check for a collision with the MainCamera to switch between the two voxel meshes

    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Trigger detected: " + other.tag + " - " + other.name);
    //     if (other.CompareTag("MainCamera") && isGeneratingOuter)
    //     {
    //         // Toggle the voxel data generation
    //         isGeneratingOuter = !isGeneratingOuter;
    //         container.ClearData();
    //         OuterComputeManager.Instance.GenerateVoxelData(ref container, ref mainCamera, isGeneratingOuter);
    //     }
    // }

    public static OuterWorldSettings WorldSettings;
    private static OuterWorldManager _instance;

    public static OuterWorldManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<OuterWorldManager>();
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