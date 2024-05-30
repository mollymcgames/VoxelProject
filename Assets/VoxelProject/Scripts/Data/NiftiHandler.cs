using System;
using System.IO;
using Nifti.NET;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiFile.Read(niftiFilePath);
    }


    public static VoxelCell[] ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        int numVoxels = width * height * depth;

        VoxelCell[] voxelValue = new VoxelCell[numVoxels];

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Access the voxel value at (x, y, z)
                    voxelValue[index] = new VoxelCell(z, y, x, niftiData.Data[index]);
                    index++;
                }
            }
        }
        return voxelValue;
    }

}