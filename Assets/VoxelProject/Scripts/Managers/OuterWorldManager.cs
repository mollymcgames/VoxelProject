using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterWorldManager : MonoBehaviour
{
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public OuterWorldSettings worldSettings;
    public VoxelCell[] sourceDataInner;

    public VoxelCell[] sourceDataOuter;

    public OuterContainer container;

    private string filepathInner = "Assets/Resources/blue.txt";
    private string filepathOuter = "Assets/Resources/voxtest.txt";

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

        OuterComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("OuterContainer");
      
        cont.transform.parent = transform;
        container = cont.AddComponent<OuterContainer>();
        container.Initialize(worldMaterial, Vector3.zero);    
        // Set the tag for collision detection
        cont.tag = "OuterContainer";
  

        // Add a collider to the container for collision detection
        BoxCollider collider = cont.AddComponent<BoxCollider>();
        collider.isTrigger = true; // Enable IsTrigger   
        collider.size = new Vector3(11, 9, 9); // Adjust the size as needed

        
        // Tag the inner container
        GameObject outerContainer = GameObject.FindWithTag("OuterContainer");
        if (outerContainer == null)
        {
            Debug.LogError("OuterContainer tag not found. Make sure the inner container is tagged correctly.");
        }

        OuterComputeManager.Instance.GenerateVoxelData(ref container, true);
        
        // Correct rotation if needed
        cont.transform.Rotate(270, 0, 0); // Adjust this as necessary to correct the orientation                
    }

    bool isGeneratingOuter = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isGeneratingOuter = !isGeneratingOuter;                
            container.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref container, isGeneratingOuter);
        }

        Debug.Log("Camera position: " + mainCamera.transform.position);
        //if the camera is a certain distance away from the container, switch to the other container
        if (!isGeneratingOuter && Vector3.Distance(mainCamera.transform.position, container.transform.position) > 18)
        {
            isGeneratingOuter = !isGeneratingOuter;
            container.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref container, isGeneratingOuter);
        }
    }
    
    //check for a collision with the MainCamera to switch between the two voxel meshes

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected: " + other.tag + " - " + other.name);
        if (other.CompareTag("MainCamera") && isGeneratingOuter)
        {
            // Toggle the voxel data generation
            isGeneratingOuter = !isGeneratingOuter;
            container.ClearData();
            OuterComputeManager.Instance.GenerateVoxelData(ref container, isGeneratingOuter);
        }
    }

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