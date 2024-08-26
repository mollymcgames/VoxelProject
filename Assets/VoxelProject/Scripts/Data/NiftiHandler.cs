using System.Collections.Generic;
using Nifti.NET;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        Nifti.NET.Nifti tempNifti = NiftiFile.Read(niftiFilePath);

        float calMin = tempNifti.Header.cal_min;
        float calMax = tempNifti.Header.cal_max;
        if (calMax <= 0)
        {
            int index = 0;
            foreach (var item in tempNifti.Data)
            {
                if (tempNifti.Data[index] < calMin)
                    calMin = tempNifti.Data[index];

                if (tempNifti.Data[index] > calMax)
                    calMax = tempNifti.Data[index];

                index++;
            }
        }
        tempNifti.Header.cal_min = calMin;
        tempNifti.Header.cal_max = calMax;
        Debug.Log("CAL MIN onload: " + tempNifti.Header.cal_min);
        Debug.Log("CAL MAX onload: " + tempNifti.Header.cal_max);

        return tempNifti;
    }

    public static Dictionary<Vector3Int, Voxel> ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth, int niiVoxelOmissionThreshold)
    {
        float calMin = niftiData.Header.cal_min;
        float calMax = niftiData.Header.cal_max;
        Debug.Log("CAL MIN: " + calMin);
        Debug.Log("CAL MAX: " + calMax);
        Debug.Log("AUX FILE: " + ByteToString(niftiData.Header.aux_file));

        int numVoxels = width * height * depth;

        Dictionary < Vector3Int, Voxel > voxelDictionary = new Dictionary<Vector3Int, Voxel> (numVoxels, new FastVector3IntComparer());

        // Iterate through each voxel
        int voxelsLoaded = 0;
        int index = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((byte)niftiData.Data[index] >= (byte)niiVoxelOmissionThreshold)
                    {                        
                        // Convert the number to a string to easily access each digit
                        // Different NII files represent colours in different ways. Decision here is to make everything in the range
                        // 0 to 254, this way greyscale will be the default but it can be turned into RGB if needed.
                        int colorGreyScale = ((int)((float)(niftiData.Data[index] / calMax) * 254) % 254);                        
                        voxelDictionary.Add(new Vector3Int(x, y, z), new Voxel(colorGreyScale));
                        voxelsLoaded++;
                    }
                    index++;
                }
            }
        }
        Debug.Log("Voxels scanned:" + index);
        Debug.Log("Voxels loaded:" + voxelsLoaded);
        Debug.Log("Voxels dropped:" + (index-voxelsLoaded).ToString());
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