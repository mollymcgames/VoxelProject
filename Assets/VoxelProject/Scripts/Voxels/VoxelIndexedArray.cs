using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class VoxelIndexedArray<T> where T : struct
{
    private bool initialized = false;

    [SerializeField]
    [HideInInspector]
    public T[] array;

    [SerializeField]
    [HideInInspector]
    private Vector3Int size;

    public VoxelIndexedArray()
    {
        Create(VoxelWorldManager.WorldSettings.maxWidthX, VoxelWorldManager.WorldSettings.maxHeightY, VoxelWorldManager.WorldSettings.maxDepthZ);
    }

    public VoxelIndexedArray(int sizeX, int sizeY, int sizeZ)
    {
        Create(sizeX, sizeY, sizeZ);
    }

    private void Create(int sizeX, int sizeY, int sizeZ)
    {
        size = new Vector3Int(sizeX, sizeY, sizeZ);
        array = new T[Count];
        initialized = true;
    }

    int IndexFromCoord(Vector3Int idx)
    {
        return idx.x + idx.y * VoxelWorldManager.WorldSettings.maxWidthX + idx.z * VoxelWorldManager.WorldSettings.maxDepthZ * VoxelWorldManager.WorldSettings.maxHeightY;
        //return idx.x + (idx.y * size.x) + (idx.z * size.x * size.y);
        //return Mathf.RoundToInt(idx.x) + (Mathf.RoundToInt(idx.y) * size.x) + (Mathf.RoundToInt(idx.z) * size.x * size.y);
    }

    public void Clear()
    {
        if (!initialized)
            return;

        Array.Clear(array, 0, array.Length);
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

    public T this[Vector3Int coord]
    {
        get
        {
            Vector3Int unityCoords = coord;

            //coord.z < 0 || coord.z > size.x)
            if (unityCoords.x < 0 || unityCoords.x > size.x ||
                unityCoords.y < 0 || unityCoords.y > size.y ||
                unityCoords.z < 0 || unityCoords.z > size.z)
            {
                //Debug.LogError($"Coordinates GET out of bounds! {coord}");
                return default(T);
            }
            int coordIndex = IndexFromCoord(coord);
            return array[coordIndex];
        }
        set
        {
            Vector3Int unityCoords = (coord);

            if (unityCoords.x < 0 || unityCoords.x >= size.x ||
                unityCoords.y < 0 || unityCoords.y >= size.y ||
                unityCoords.z < 0 || unityCoords.z >= size.z)
            {
                Debug.LogError($"Coordinates SET out of bounds! {coord}");
                return;
            }
            int coordIndex = IndexFromCoord(coord);
            array[coordIndex] = value;
        }
    }

}
