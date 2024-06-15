using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class VoxelComputeManager : MonoBehaviour
{
    public ComputeShader noiseShader;

    private List<VoxelNoiseBuffer> allNoiseComputeBuffers = new List<VoxelNoiseBuffer>();
    private Queue<VoxelNoiseBuffer> availableNoiseComputeBuffers = new Queue<VoxelNoiseBuffer>();

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
    public VoxelNoiseBuffer GetNoiseBuffer()
    {
        if (availableNoiseComputeBuffers.Count > 0)
            return availableNoiseComputeBuffers.Dequeue();
        else
        {
            return CreateNewNoiseBuffer(false);
        }
    }

    public VoxelNoiseBuffer CreateNewNoiseBuffer(bool enqueue = true)
    {
        VoxelNoiseBuffer buffer = new VoxelNoiseBuffer();
        buffer.InitializeBuffer();
        allNoiseComputeBuffers.Add(buffer);

        if (enqueue)
            availableNoiseComputeBuffers.Enqueue(buffer);

        return buffer;
    }

    public void ClearAndRequeueBuffer(VoxelNoiseBuffer buffer)
    {
        ClearVoxelData(buffer);
        availableNoiseComputeBuffers.Enqueue(buffer);
    }
    #endregion

    #region Compute Helpers

    public void GenerateVoxelData(ref VoxelContainer container, ref Camera mainCamera, bool renderOuter = false)
    {
        // Do not attempt to schedule rendering of a mesh if the container is null or the unity game is currently quitting
        if (OuterWorldManager.Instance != null && OuterWorldManager.Instance.containerOuter != null && !OuterWorldManager.Instance.quitting)
        {
            noiseShader.SetBuffer(0, "voxelArray", container.data.noiseBuffer);
            noiseShader.SetBuffer(0, "count", container.data.countBuffer);

            noiseShader.SetVector("chunkPosition", container.containerPosition);
            noiseShader.SetVector("seedOffset", Vector3.zero);

            noiseShader.Dispatch(0, xThreads, yThreads, xThreads);
            noiseShader.Dispatch(0, xThreads, yThreads, zThreads);

            float transparencyValue = AdjustMaterialTransparency(ref container, ref mainCamera);
            Debug.Log("New Transparency Value: " + transparencyValue);

            AsyncGPUReadback.Request(container.data.noiseBuffer, (callback) =>
            {
                if (OuterWorldManager.Instance != null && OuterWorldManager.Instance.containerOuterDic != null)
                {
                    callback.GetData<Voxel>(0).CopyTo(OuterWorldManager.Instance.containerOuterDic.data.voxelArray.array);
                    OuterWorldManager.Instance.containerOuterDic.RenderMeshDictionary(true, transparencyValue);
                }
            });

        }
    }


    private float AdjustMaterialTransparency(ref VoxelContainer container, ref Camera mainCamera)
    {
        float distance = Vector3.Distance(mainCamera.transform.position, container.transform.position);
        float maxDistance = 30f;
        float minDistance = 10f;

        // Adjust the transparency of the material based on the camera distance
        // alpha should be 0 if close and 1 if far
        float alpha = Mathf.InverseLerp(minDistance, maxDistance, distance);

        if (container != null)
        {
            Debug.Log("Adjusting transparency with alpha: " + alpha);
        }
        else
        {
            Debug.LogWarning("No Renderer found on the container.");
        }

        return alpha;
    }

    private void ClearVoxelData(VoxelNoiseBuffer buffer)
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
        foreach (VoxelNoiseBuffer buffer in allNoiseComputeBuffers)
            buffer.Dispose();
    }


    private static VoxelComputeManager _instance;

    public static VoxelComputeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<VoxelComputeManager>();
            return _instance;
        }
    }
}