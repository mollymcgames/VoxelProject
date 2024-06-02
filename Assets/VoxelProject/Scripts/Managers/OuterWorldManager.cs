using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterWorldManager : MonoBehaviour
{
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public OuterWorldSettings worldSettings;
    public VoxelCell[] sourceData;

    public OuterContainer container;

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

        sourceData = SourceDataTextFileLoader.LoadSourceData();
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataTextFileLoader.widthX;
        WorldSettings.maxHeightY = SourceDataTextFileLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataTextFileLoader.depthZ;

        OuterComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("OuterContainer");
        cont.transform.parent = transform;
        container = cont.AddComponent<OuterContainer>();
        container.Initialize(worldMaterial, Vector3.zero);    

        OuterComputeManager.Instance.GenerateVoxelData(ref container);

        // // Correct rotation if needed
        cont.transform.Rotate(270, 0, 0); // Adjust this as necessary to correct the orientation        
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