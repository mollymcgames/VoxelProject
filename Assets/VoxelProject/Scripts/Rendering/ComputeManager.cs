using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
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
    
    private Container voxelContainer;

    public void Initialize(int count = 256)
    {
        xThreads = WorldManager.Instance.worldSettings.maxWidthX / 8 + 1;
        yThreads = WorldManager.Instance.worldSettings.maxHeightY / 8;
        zThreads = WorldManager.Instance.worldSettings.maxDepthZ / 8;

        Debug.Log("XTHREADS: " + xThreads);
        Debug.Log("YTHREADS: " + yThreads);
        Debug.Log("ZTHREADS: " + zThreads);

        noiseShader.SetInt("containerSizeX", WorldManager.Instance.worldSettings.maxWidthX);
        noiseShader.SetInt("containerSizeY", WorldManager.Instance.worldSettings.maxHeightY);
        noiseShader.SetInt("containerSizeZ", WorldManager.Instance.worldSettings.maxDepthZ);

        for (int i = 0; i < count; i++)
        {
            CreateNewNoiseBuffer();
        }
    }

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

    public void GenerateVoxelData(ref Container cont, int layer)
    {
        if (cont.data.noiseBuffer == null)
        {
            ComputeManager.Instance.Initialize(1);
        }

        voxelContainer = cont;
        noiseShader.SetBuffer(0, "voxelArray", cont.data.noiseBuffer);
        noiseShader.SetBuffer(0, "count", cont.data.countBuffer);

        noiseShader.SetVector("chunkPosition", cont.containerPosition);
        noiseShader.SetVector("seedOffset", Vector3.zero);

        noiseShader.Dispatch(0, xThreads, yThreads, xThreads);
        noiseShader.Dispatch(0, xThreads, yThreads, zThreads);

        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            callback.GetData<VoxelOriginal>(0).CopyTo(SCManager.Instance.container.data.voxelArray);
            voxelContainer.RenderMesh();
        });    
    }

    public void RefreshVoxels(ref Container cont, int layer)
    {
        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            callback.GetData<VoxelOriginal>(0).CopyTo(SCManager.Instance.container.data.voxelArray);
            voxelContainer.ReRenderMesh();
        });
    }

    private void ClearVoxelData(NoiseBuffer buffer)
    {
        buffer.countBuffer.SetData(new int[] { 0 });
        noiseShader.SetBuffer(1, "voxelArray", buffer.noiseBuffer);
        noiseShader.Dispatch(1, xThreads, yThreads, zThreads);
    }

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
    public VoxelOriginal[] voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        voxelArray = new VoxelOriginal[WorldManager.Instance.voxelDictionary.Count];
        noiseBuffer = new ComputeBuffer(voxelArray.Length, Marshal.SizeOf(typeof(VoxelOriginal)));
        noiseBuffer.SetData(voxelArray);
        Initialized = true;
    }

    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }

    public VoxelOriginal this[int index]
    {
        get
        {
            return voxelArray[index];
        }

        set
        {
            voxelArray[index] = value;
        }
    }
}