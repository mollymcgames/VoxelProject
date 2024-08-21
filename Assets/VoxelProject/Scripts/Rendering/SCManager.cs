using System.Collections;
using UnityEngine;

public class SCManager : MonoBehaviour
{
    private float updateInterval = 0.1f;
    public bool isZooming = false;
    public bool reRenderingMesh = false;

    [HideInInspector]
    public Container container;
    
    private static SCManager _instance;

    public static SCManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<SCManager>();
            return _instance;
        }
    }

    void Start() 
    {
        ComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(WorldManager.Instance.worldMaterial, Vector3.zero);        

        ComputeManager.Instance.GenerateVoxelData(ref container, 0);
    }

    void Update(){
        // key is 0 for now, will be changed to a more appropriate key later
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("0 key pressed, regenerating voxel data.");
            ComputeManager.Instance.RefreshVoxels(ref container, 0);
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 key pressed, regenerating voxel data.");
            ComputeManager.Instance.RefreshVoxels(ref container, 1);
        }

        // Only auto update if toggle is on
        if (WorldManager.Instance.worldSettings.autoRefresh && SCManager.Instance.reRenderingMesh == false)
        {
            // Call the custom update method
            ComputeManager.Instance.GenerateVoxelData(ref container, 0);
        }
    }
}