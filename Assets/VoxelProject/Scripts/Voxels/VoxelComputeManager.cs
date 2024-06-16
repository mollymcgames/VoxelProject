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

    private int xThreads=0;
    private int yThreads=0;
    private int zThreads=0;

    public void Initialise(int count = 256)
    {
        Debug.Log("INIT x:" + VoxelWorldManager.WorldSettings.maxWidthX);
        Debug.Log("INIT y:" + VoxelWorldManager.WorldSettings.maxHeightY);
        Debug.Log("INIT z:" + VoxelWorldManager.WorldSettings.maxDepthZ);

        xThreads = VoxelWorldManager.WorldSettings.maxWidthX / 8 + 1;
        yThreads = VoxelWorldManager.WorldSettings.maxHeightY / 8;
        zThreads = VoxelWorldManager.WorldSettings.maxDepthZ / 8;
        if (xThreads <= 0)
            xThreads = 1;
        if (yThreads <= 0)
            yThreads = 1;
        if (zThreads <= 0 )
            zThreads = 1;

        Debug.Log("POST INIT x:" + xThreads);
        Debug.Log("POST INIT y:" + yThreads);
        Debug.Log("POST INIT z:" + zThreads);

        noiseShader.SetInt("containerSizeX", VoxelWorldManager.WorldSettings.maxWidthX);
        noiseShader.SetInt("containerSizeY", VoxelWorldManager.WorldSettings.maxHeightY);
        noiseShader.SetInt("containerSizeZ", VoxelWorldManager.WorldSettings.maxDepthZ);

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

    //public void GenerateVoxelData(ref VoxelContainer container, ref Camera mainCamera)
    public void GenerateVoxelData(ref VoxelContainer container)
    {
        // Do not attempt to schedule rendering of a mesh if the container is null or the unity game is currently quitting
        if (VoxelWorldManager.Instance != null && VoxelWorldManager.Instance.voxelMeshContainer != null && !VoxelWorldManager.Instance.quitting)
        {
            noiseShader.SetBuffer(0, "voxelArray", container.data.noiseBuffer);
            noiseShader.SetBuffer(0, "count", container.data.countBuffer);

            noiseShader.SetVector("chunkPosition", (Vector3)container.containerPosition);
            noiseShader.SetVector("seedOffset", Vector3.zero);

/*            Debug.Log("World xThreads:" + xThreads);
            Debug.Log("World yThreads:" + yThreads);
            Debug.Log("World zThreads:" + zThreads);*/

            noiseShader.Dispatch(0, xThreads, yThreads, xThreads);
            noiseShader.Dispatch(0, xThreads, yThreads, zThreads);

            float transparencyValue = AdjustMaterialTransparency(ref container);
            //Debug.Log("New Transparency Value: " + transparencyValue);

            AsyncGPUReadback.Request(container.data.noiseBuffer, (callback) =>
            {
                // Do not attempt to render anything if the WorldManager isn't ready yet!
                if (VoxelWorldManager.Instance != null && VoxelWorldManager.Instance.voxelMeshContainer != null)
                {
                    callback.GetData<Voxel>(0).CopyTo(VoxelWorldManager.Instance.voxelMeshContainer.data.voxelArray.array);
                    VoxelWorldManager.Instance.voxelMeshContainer.RenderMesh(transparencyValue);
                }
            });

        }
    }


    private float AdjustMaterialTransparency(ref VoxelContainer container)
    {
        float distance = Vector3.Distance(Camera.main.transform.position, container.transform.position);
        float maxDistance = 30f;
        float minDistance = 10f;

        // Adjust the transparency of the material based on the camera distance
        // alpha should be 0 if close and 1 if far
        float alpha = Mathf.InverseLerp(minDistance, maxDistance, distance);

/*        if (container != null)
        {
            //Debug.Log("Adjusting transparency with alpha: " + alpha);
        }
        else
        {
            Debug.LogWarning("No Renderer found on the container.");
        }
*/
        return alpha;
    }

    private void ClearVoxelData(VoxelNoiseBuffer buffer)
    {
/*        Debug.Log("World CLEAR xThreads:" + xThreads);
        Debug.Log("World CLEAR yThreads:" + yThreads);
        Debug.Log("World CLEAR zThreads:" + zThreads);*/

        if (xThreads==0 || yThreads==0 || zThreads==0)
        { 
            return; 
        }

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