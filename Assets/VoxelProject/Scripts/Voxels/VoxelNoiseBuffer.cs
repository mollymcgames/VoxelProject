using System.Runtime.InteropServices;
using UnityEngine;

public struct VoxelNoiseBuffer
{
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;
    public bool Initialized;
    public bool Cleared;
    public VoxelIndexedArray<VoxelStruct> voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        voxelArray = new VoxelIndexedArray<VoxelStruct>();
        //noiseBuffer = new ComputeBuffer(voxelArray.Count, 8);
        noiseBuffer = new ComputeBuffer(voxelArray.Count, Marshal.SizeOf(typeof(VoxelStruct)));

        noiseBuffer.SetData(voxelArray.GetData);
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
            return voxelArray[index];
        }

        set
        {
            voxelArray[index] = value;
        }
    }
}