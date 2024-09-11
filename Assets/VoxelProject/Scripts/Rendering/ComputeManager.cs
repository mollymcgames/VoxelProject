using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.Rendering;

//Heavily based upon original tutorial code
//Important class to manage the compute shader and the buffers used to generate voxel data
public class ComputeManager : MonoBehaviour
{
    public ComputeShader noiseShader;

    private List<NoiseBuffer> allNoiseComputeBuffers = new List<NoiseBuffer>();
    private Queue<NoiseBuffer> availableNoiseComputeBuffers = new Queue<NoiseBuffer>();

    private int xThreads;
    private int yThreads;
    private int zThreads;
    
    private Container voxelContainer; //Reference to the container holding the voxel data

    public void Initialize(int count = 256)
    {
        //Calculate the number of threads per dimension for the compute shader
        xThreads = WorldManager.Instance.worldSettings.maxWidthX / 8 + 1;
        yThreads = WorldManager.Instance.worldSettings.maxHeightY / 8;
        zThreads = WorldManager.Instance.worldSettings.maxDepthZ / 8;

        //Set the voxel container dimensions for the compute shader
        noiseShader.SetInt("containerSizeX", WorldManager.Instance.worldSettings.maxWidthX);
        noiseShader.SetInt("containerSizeY", WorldManager.Instance.worldSettings.maxHeightY);
        noiseShader.SetInt("containerSizeZ", WorldManager.Instance.worldSettings.maxDepthZ);

        //Create the initial noise buffers
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

    //Clear the voxel data in the buffer
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
    
    //Disposes all buffers to free up GPU memory when the application is closed
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

    //Disposes the buffers to free up GPU memory when they're no longer needed
    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }

    //Indexer to access individual voxel data by index
    public VoxelOriginal this[int index]
    {
        get
        {
            return voxelArray[index]; //Get voxel at specified index
        }

        set
        {
            voxelArray[index] = value; //Set voxel at specified index
        }
    }
}