using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    public VoxelColor[] WorldColors;
    public WorldSettings worldSettings;
    public VoxelCell[] sourceData;

    public Container container;

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

        sourceData = SourceDataLoader.LoadSourceData();
        WorldSettings = worldSettings;
        WorldSettings.maxWidthX = SourceDataLoader.widthX;
        WorldSettings.maxHeightY = SourceDataLoader.heightY;
        WorldSettings.maxDepthZ = SourceDataLoader.depthZ;

        ComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(worldMaterial, MenuHandler.containerPosition); //Use the menu handler to set the container position

        ComputeManager.Instance.GenerateVoxelData(ref container);
    }

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
}

[System.Serializable]
public class WorldSettings
{
    public int containerSize = 16;
    public int maxHeight = 128;
    public int maxWidthX = 0;
    public int maxDepthZ = 0;
    public int maxHeightY = 0;
}