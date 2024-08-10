using System.Collections.Generic;
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


    // D AS L public static Dictionary<long, VoxelCell> ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    public static Dictionary<Vector3Int, VoxelCell> ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        float calMin = niftiData.Header.cal_min;
        float calMax = niftiData.Header.cal_max;
        Debug.Log("CAL MIN: " + calMin);
        Debug.Log("CAL MAX: " + calMax);
        Debug.Log("AUX FILE: " + ByteToString(niftiData.Header.aux_file));

        int numVoxels = width * height * depth;

        // FIXP VoxelCell[,,] voxelDictionary = new VoxelCell[niftiData.Dimensions[0], niftiData.Dimensions[1], niftiData.Dimensions[2]];
        Dictionary < Vector3Int, VoxelCell > voxelDictionary = new Dictionary<Vector3Int, VoxelCell> (numVoxels);
        // D AS L Dictionary < long, VoxelCell > voxelDictionary = new Dictionary<long, VoxelCell> (numVoxels);

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Convert the number to a string to easily access each digit
                    // Different NII files represent colours in different ways. Decision here is to make everything in the range
                    // 0 to 254, this way greyscale will be the default but it can be turned into RGB if needed.
                    string color = ( (int)((float)(niftiData.Data[index++]/calMax)*254) % 254).ToString();
                    // D AS L voxelDictionary.Add(Vector3IntConvertor.EncodeVector3Int(new Vector3Int(x, y, z)), new VoxelCell(z, y, x, color));
                    voxelDictionary.Add(new Vector3Int(x, y, z), new VoxelCell(z, y, x, color));
                }
            }
        }
        return voxelDictionary;
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