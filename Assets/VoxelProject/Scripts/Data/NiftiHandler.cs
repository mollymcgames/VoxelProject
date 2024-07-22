using System;
using System.IO;
using Nifti.NET;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        Nifti.NET.Nifti tempNifti = NiftiFile.Read(niftiFilePath);

        float calMax = tempNifti.Header.cal_max;
        if (calMax <= 0)
        {
            int index = 0;
            foreach (var item in tempNifti.Data)
            {
                if (calMax < tempNifti.Data[index])
                    calMax = tempNifti.Data[index];
                index++;
            }
        }
        tempNifti.Header.cal_max = calMax;
        Debug.Log("CAL MAX onload: " + tempNifti.Header.cal_max);

        return tempNifti;
    }


    public static VoxelCell[] ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        float calMin = niftiData.Header.cal_min;
        float calMax = niftiData.Header.cal_max;
        Debug.Log("CAL MIN: " + calMin);
        Debug.Log("CAL MAX: " + calMax);
        Debug.Log("AUX FILE: " + ByteToString(niftiData.Header.aux_file));

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
                    //voxelValue[index] = new VoxelCell(z, y, x, niftiData.Data[index].ToString());

                    // Convert the number to a string to easily access each digit
                    // Different NII files represent colours in different ways. Decision here is to make everything in the range
                    // 0 to 254, this way greyscale will be the default but it can be turned into RGB if needed.
                    voxelValue[index] = new VoxelCell(z, y, x, ((int)(niftiData.Data[index] % 255)).ToString());
                    index++;
                }
            }
        }
        return voxelValue;
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
}