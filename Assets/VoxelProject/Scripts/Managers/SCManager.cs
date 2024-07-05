using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SCManager : MonoBehaviour
{
    public Container container;

    void Start() 
    {
        ComputeManager.Instance.Initialize(1);
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        Debug.Log("SCManager Start - transform:"+cont.transform.parent);
        container = cont.AddComponent<Container>();
        container.Initialize(WorldManager.Instance.worldMaterial, Vector3.zero);

        ComputeManager.Instance.GenerateVoxelData(ref container, 0);
    }

    void Update(){
        // key is 0 for now, will be changed to a more appropriate key later
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("0 key pressed, generating voxel data.");
            ComputeManager.Instance.GenerateVoxelData(ref container, 0);
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 key pressed, generating voxel data.");
            ComputeManager.Instance.GenerateVoxelData(ref container, 1);
        }
    }
}