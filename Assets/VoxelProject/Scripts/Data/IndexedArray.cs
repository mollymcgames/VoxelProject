using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class IndexedArray<T> where T : struct
{
    private bool initialized = false;

    [SerializeField]
    [HideInInspector]
    public T[] array;

    [SerializeField]
    [HideInInspector]
    private Vector3Int size;

    public static readonly float MillimetersToUnityScale = 0.001f; // 1 millimeter = 0.001 Unity units


    public IndexedArray()
    {
//        Create(WorldManager.WorldSettings.containerSize, WorldManager.WorldSettings.maxHeight);
        Create(WorldManager.WorldSettings.maxWidthX, WorldManager.WorldSettings.maxHeightY, WorldManager.WorldSettings.maxDepthZ);
    }

    public IndexedArray(int sizeX, int sizeY, int sizeZ)
    {
        Create(sizeX, sizeY, sizeZ);
    }

    private void Create(int sizeX, int sizeY, int sizeZ)
    {
        //size = new Vector3Int(sizeX + 3, sizeY + 1, sizeZ + 3);
        size = new Vector3Int(sizeX, sizeY, sizeZ);
        array = new T[Count];
        initialized = true;
    }


    int IndexFromCoord(Vector3 idx)
    {
        return Mathf.RoundToInt(idx.x) + (Mathf.RoundToInt(idx.y) * size.x) + (Mathf.RoundToInt(idx.z) * size.x * size.y);
    }

    public void Clear()
    {
        if (!initialized)
            return;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                for (int z = 0; z < size.z; z++)
                //for (int z = 0; z < size.x; z++)
                    array[x + (y * size.x) + (z * size.x * size.y)] = default(T);
    }

    public int Count
    {
        get { return size.x * size.y * size.z; }
    }

    public T[] GetData
    {
        get
        {
            return array;
        }
    }

    // Convert millimeter coordinates to Unity scale
    private Vector3 ConvertToUnityScale(Vector3 mmCoords)
    {
        return mmCoords * MillimetersToUnityScale;
    }    

    public T this[Vector3 coord]
    {
        get
        {
            Vector3 unityCoords = ConvertToUnityScale(coord);

            //coord.z < 0 || coord.z > size.x)
            if (unityCoords.x < 0 || unityCoords.x > size.x ||
                unityCoords.y < 0 || unityCoords.y > size.y ||
                unityCoords.z < 0 || unityCoords.z > size.z)
            {
                Debug.LogError($"Coordinates GET out of bounds! {coord}");
                return default(T);
            }
            return array[IndexFromCoord(coord)];
        }
        set
        {
            Vector3 unityCoords = ConvertToUnityScale(coord);

            if (unityCoords.x < 0 || unityCoords.x >= size.x ||
                unityCoords.y < 0 || unityCoords.y >= size.y ||
                unityCoords.z < 0 || unityCoords.z >= size.z)
            {
                Debug.LogError($"Coordinates SET out of bounds! {coord}");
                return;
            }
            array[IndexFromCoord(coord)] = value;
        }
    }

}
