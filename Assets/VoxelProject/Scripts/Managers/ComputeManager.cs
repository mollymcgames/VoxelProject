using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader noiseShader;

    private List<NoiseBuffer> allNoiseComputeBuffers = new List<NoiseBuffer>();
    private Queue<NoiseBuffer> availableNoiseComputeBuffers = new Queue<NoiseBuffer>();

    private int xThreads;
    private int yThreads;
    private int zThreads;

    public void Initialize(int count = 256)
    {
        xThreads = WorldManager.Instance.worldSettings.maxWidthX / 8 + 1;
        yThreads = WorldManager.Instance.worldSettings.maxHeightY / 8;
        zThreads = WorldManager.Instance.worldSettings.maxDepthZ / 8;
        
        noiseShader.SetInt("containerSizeX", WorldManager.Instance.worldSettings.maxWidthX);
        noiseShader.SetInt("containerSizeY", WorldManager.Instance.worldSettings.maxHeightY);
        noiseShader.SetInt("containerSizeZ", WorldManager.Instance.worldSettings.maxDepthZ);

        for (int i = 0; i < count; i++)
        {
            CreateNewNoiseBuffer();
        }
    }

    public Camera mainCamera;

    #region Noise Buffers

    #region Pooling
    public NoiseBuffer GetNoiseBuffer()
    {
        if (availableNoiseComputeBuffers.Count > 0)
            return availableNoiseComputeBuffers.Dequeue();
        else
        {
            return CreateNewNoiseBuffer(false);
        }
    }

    public NoiseBuffer CreateNewNoiseBuffer(bool enqueue = true)
    {
        NoiseBuffer buffer = new NoiseBuffer();
        buffer.InitializeBuffer();
        allNoiseComputeBuffers.Add(buffer);

        if (enqueue)
            availableNoiseComputeBuffers.Enqueue(buffer);

        return buffer;
    }

    public void ClearAndRequeueBuffer(NoiseBuffer buffer)
    {
        ClearVoxelData(buffer);
        availableNoiseComputeBuffers.Enqueue(buffer);
    }
    #endregion

    #region Compute Helpers

    public void GenerateVoxelData(ref Container cont, int layer)
    {
        Container container = cont;
        noiseShader.SetBuffer(0, "voxelArray", cont.data.noiseBuffer);
        noiseShader.SetBuffer(0, "count", cont.data.countBuffer);

        noiseShader.SetVector("chunkPosition", cont.containerPosition);
        noiseShader.SetVector("seedOffset", Vector3.zero);

        noiseShader.Dispatch(0, xThreads, yThreads, xThreads);
        noiseShader.Dispatch(0, xThreads, yThreads, zThreads);

        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            container.RenderVisibleChunks(layer);
        });    
    }

    private void ClearVoxelData(NoiseBuffer buffer)
    {
        buffer.countBuffer.SetData(new int[] { 0 });
        noiseShader.SetBuffer(1, "voxelArray", buffer.noiseBuffer);
        noiseShader.Dispatch(1, xThreads, yThreads, zThreads);
    }
    #endregion
    #endregion

    private void OnApplicationQuit()
    {
        DisposeAllBuffers();
    }

    public void DisposeAllBuffers()
    {
        foreach (NoiseBuffer buffer in allNoiseComputeBuffers)
            buffer.Dispose();
    }


    private static ComputeManager _instance;

    public static ComputeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<ComputeManager>();
            return _instance;
        }
    }
}

public struct NoiseBuffer
{
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;
    public bool Initialized;
    public bool Cleared;
    //public IndexedArray<Voxel> voxelArray;
    public VoxelStruct[,,] voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        //voxelArray = new IndexedArray<Voxel>();
        Debug.Log("Initialising noise buffer with world settings: " + WorldManager.Instance.worldSettings.maxWidthX + ", " + WorldManager.Instance.worldSettings.maxHeightY + ", " + WorldManager.Instance.worldSettings.maxDepthZ);
        voxelArray = new VoxelStruct[WorldManager.Instance.worldSettings.maxWidthX, WorldManager.Instance.worldSettings.maxHeightY, WorldManager.Instance.worldSettings.maxDepthZ];
        Debug.Log("Initialising noise buffer with size: " + voxelArray.Length);
        //noiseBuffer = new ComputeBuffer(voxelArray.Length, Marshal.SizeOf(typeof(Voxel)));  
        noiseBuffer = new ComputeBuffer(voxelArray.Length, Marshal.SizeOf(typeof(VoxelStruct)));  
        Debug.Log("Initialised noise buffer with size: " + noiseBuffer.count);
        //noiseBuffer.SetData(voxelArray.GetData);
        noiseBuffer.SetData(voxelArray);
        Initialized = true;
    }

    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }
    public VoxelStruct this[Vector3Int index]
    {
        get
        {
            //return voxelArray[index];
            return voxelArray[index.x,index.y,index.z];
        }

        set
        {
            //voxelArray[index] = value;
            voxelArray[index.x, index.y, index.z] = value;            
        }
    }
}