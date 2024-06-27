using System;
using System.IO;
using Nifti.NET;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static int visibleColourThreshold = 18;

    public static Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiFile.Read(niftiFilePath);
    }

    public static Voxel[,,] ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        int numVoxels = width * height * depth;

        Voxel[,,] voxelValue = new Voxel[width,height,depth];

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Don't even bother with a voxel if it's "dark!"
                    if ((int)niftiData.Data[index] < visibleColourThreshold)
                    {
                        index++;
                        continue;
                    }
                    voxelValue[x, y, z] = new Voxel( (int)niftiData.Data[index] > visibleColourThreshold, (int)niftiData.Data[index]); //, x, niftiData.Data[index].ToString());
                    index++;
                }
            }
        }
        return voxelValue;
    }


    /* oldway    public static VoxelCell[] ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
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
                        voxelValue[index] = new VoxelCell(z, y, x, niftiData.Data[index].ToString());
                        index++;
                    }
                }
            }
            return voxelValue;
        }
    */
}