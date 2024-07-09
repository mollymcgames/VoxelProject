using System;
using System.IO;
using Nifti.NET;
using Unity.VisualScripting;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static int visibleColourThreshold = 1;

    public static Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiFile.Read(niftiFilePath);
    }

    private static string ByteToString(byte[] source)
    {
        try
        {
            return source != null ? System.Text.Encoding.UTF8.GetString(source) : "";
        }
        catch
        {
            return "";
        }

    }

    public static Voxel[,,] ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        Debug.Log("CAL MIN: " + niftiData.Header.cal_min);
        Debug.Log("CAL MAX: " + niftiData.Header.cal_max);
        Debug.Log("AUX FILE: " + ByteToString(niftiData.Header.aux_file));

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
                    // FORCE RED FOR NOW
                    if (x > 10 && x < 30)
                    {                        
                        voxelValue[x, y, z] = new Voxel((int)niftiData.Data[index] > visibleColourThreshold, false, Convert.ToInt32("#FF0000".Replace("#", ""), 16)); //, x, niftiData.Data[index].ToString());
                    }
                    else
                    {
                        //Debug.Log("Next vox colour=" + (int)niftiData.Data[index]);
                        voxelValue[x, y, z] = new Voxel((int)niftiData.Data[index] > visibleColourThreshold, true, (int)niftiData.Data[index]); //, x, niftiData.Data[index].ToString());
                    }                    
                    index++;
                }
            }
        }
        return voxelValue;
    }

    public static Voxel[,,] ReadNiftiSegmentData(ref Voxel[,,] voxelData, int segmentLayer, Nifti.NET.Nifti niftiFileLines, int widthX, int heightY, int depthZ)
    {
        int numVoxels = widthX * heightY * depthZ;

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depthZ; z++)
        {
            for (int y = 0; y < heightY; y++)
            {
                for (int x = 0; x < widthX; x++)
                {
                    //is voxel data at x,y position already set?
                    //if (voxelData[x, y, z].isSolid == true)
                    //{
                        voxelData[x, y, z].addColourRGBLayer(segmentLayer, (int)niftiFileLines.Data[index]);
                        index++;
                    //}
                    // else 
                    // {
                    //     voxelData[x, y, z] = new Voxel((int)niftiFileLines.Data[index] > visibleColourThreshold, segmentLayer, (int)niftiFileLines.Data[index]); //, x, niftiFileLines.Data[index].ToString());
                    //     index++;
                    // }
                }
            }
        }
        return voxelData;
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