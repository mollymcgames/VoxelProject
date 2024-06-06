using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OuterComputeManager : MonoBehaviour
{
    public ComputeShader noiseShader;

    private List<OuterNoiseBuffer> allNoiseComputeBuffers = new List<OuterNoiseBuffer>();
    private Queue<OuterNoiseBuffer> availableNoiseComputeBuffers = new Queue<OuterNoiseBuffer>();

    private int xThreads;
    private int yThreads;
    private int zThreads;

    public void Initialize(int count = 256)
    {
        xThreads = OuterWorldManager.WorldSettings.maxWidthX / 8 + 1;
        yThreads = OuterWorldManager.WorldSettings.maxHeightY / 8;
        zThreads = OuterWorldManager.WorldSettings.maxDepthZ / 8;

        noiseShader.SetInt("containerSizeX", OuterWorldManager.WorldSettings.maxWidthX);
        noiseShader.SetInt("containerSizeY", OuterWorldManager.WorldSettings.maxHeightY);
        noiseShader.SetInt("containerSizeZ", OuterWorldManager.WorldSettings.maxDepthZ);

        for (int i = 0; i < count; i++)
        {
            CreateNewNoiseBuffer();
        }
    }

    #region Noise Buffers

    #region Pooling
    public OuterNoiseBuffer GetNoiseBuffer()
    {
        if (availableNoiseComputeBuffers.Count > 0)
            return availableNoiseComputeBuffers.Dequeue();
        else
        {
            return CreateNewNoiseBuffer(false);
        }
    }

    public OuterNoiseBuffer CreateNewNoiseBuffer(bool enqueue = true)
    {
        OuterNoiseBuffer buffer = new OuterNoiseBuffer();
        buffer.InitializeBuffer();
        allNoiseComputeBuffers.Add(buffer);

        if (enqueue)
            availableNoiseComputeBuffers.Enqueue(buffer);

        return buffer;
    }

    public void ClearAndRequeueBuffer(OuterNoiseBuffer buffer)
    {
        ClearVoxelData(buffer);
        availableNoiseComputeBuffers.Enqueue(buffer);
    }
    #endregion

    #region Compute Helpers

    public void GenerateVoxelData(ref OuterContainer cont, bool renderOuter = false)
    {
        noiseShader.SetBuffer(0, "voxelArray", cont.data.noiseBuffer);
        noiseShader.SetBuffer(0, "count", cont.data.countBuffer);

        noiseShader.SetVector("chunkPosition", cont.containerPosition);
        noiseShader.SetVector("seedOffset", Vector3.zero);

        noiseShader.Dispatch(0, xThreads, yThreads, xThreads);
        noiseShader.Dispatch(0, xThreads, yThreads, zThreads);

        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            callback.GetData<Voxel>(0).CopyTo(OuterWorldManager.Instance.container.data.voxelArray.array);
            OuterWorldManager.Instance.container.RenderMesh(renderOuter);
        });    
    }

    private void ClearVoxelData(OuterNoiseBuffer buffer)
    {
        buffer.countBuffer.SetData(new int[] { 0 });
        noiseShader.SetBuffer(1, "voxelArray", buffer.noiseBuffer);
        //noiseShader.Dispatch(1, xThreads, yThreads, xThreads);
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
        foreach (OuterNoiseBuffer buffer in allNoiseComputeBuffers)
            buffer.Dispose();
    }


    private static OuterComputeManager _instance;

    public static OuterComputeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<OuterComputeManager>();
            return _instance;
        }
    }
}

public struct OuterNoiseBuffer
{
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;
    public bool Initialized;
    public bool Cleared;
    public OuterIndexedArray<Voxel> voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        voxelArray = new OuterIndexedArray<Voxel>();
        noiseBuffer = new ComputeBuffer(voxelArray.Count, 4);
        noiseBuffer.SetData(voxelArray.GetData);
        Initialized = true;
    }

    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }
    public Voxel this[Vector3 index]
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