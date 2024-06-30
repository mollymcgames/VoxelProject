using UnityEngine;

public struct VoxelNoiseBuffer
{
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;
    public bool Initialized;
    public bool Cleared;
    public VoxelIndexedArray<Voxel> voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        voxelArray = new VoxelIndexedArray<Voxel>();
        noiseBuffer = new ComputeBuffer(voxelArray.Count, 8);
        noiseBuffer.SetData(voxelArray.GetData);
        Initialized = true;
    }

    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }

    public Voxel this[Vector3Int index]
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