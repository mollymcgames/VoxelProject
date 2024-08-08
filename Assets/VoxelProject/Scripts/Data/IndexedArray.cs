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

    public IndexedArray()
    {
        Create(WorldManager.WorldSettings.widthX, WorldManager.WorldSettings.heightY, WorldManager.WorldSettings.depthZ);
    }

    public IndexedArray(int sizeX, int sizeY, int sizeZ)
    {
        Create(sizeX, sizeY, sizeZ);
    }

    private void Create(int sizeX, int sizeY, int sizeZ)
    {
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

    public T this[Vector3 coord]
    {
        get
        {
            Vector3 unityCoords = coord;

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
            Vector3 unityCoords = (coord);

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
