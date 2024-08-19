using UnityEngine;

public class WorldManagerV1 : MonoBehaviour
{
    public Material worldMaterial;
    
    public VoxelColor[] WorldColors;
    public WorldSettings worldSettings;
    public Voxel[] sourceData;

    public Container container;

    public static WorldSettings WorldSettings;
    private static WorldManagerV1 _instance;

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

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        ComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(worldMaterial, Vector3.zero);
    }

    public static WorldManagerV1 Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<WorldManagerV1>();
            return _instance;
        }
    }
}

[System.Serializable]
public class WorldSettingsV1
{
    public int containerSize = 16;
    public int maxHeight = 128;
    public int maxWidthX = 0;
    public int maxDepthZ = 0;
    public int maxHeightY = 0;
}