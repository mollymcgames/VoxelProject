using System.Collections.Generic;
using Nifti.NET;
using UnityEngine;

public class NiftiHandler
{
    private Nifti.NET.Nifti niftiFile = null;

    public Nifti.NET.Nifti ReadNiftiFileOnly(string niftiFilePath)
    {
        if (niftiFilePath != null && niftiFile == null)
        {
            // Load the NIfTI file
            niftiFile = NiftiFile.Read(niftiFilePath);
        }
        return niftiFile;
    }

    public Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {        
        if (niftiFilePath != null && niftiFile == null)
        {
            ReadNiftiFileOnly(niftiFilePath);
        }

        float calMin = niftiFile.Header.cal_min;
        float calMax = niftiFile.Header.cal_max;
        if (calMax <= 0)
        {
            int index = 0;
            foreach (var item in niftiFile.Data)
            {
                if (niftiFile.Data[index] < calMin)
                    calMin = (float)niftiFile.Data[index];

                if (niftiFile.Data[index] > calMax)
                    calMax = (float)niftiFile.Data[index];

                index++;
            }
        }
        niftiFile.Header.cal_min = calMin;
        niftiFile.Header.cal_max = calMax;
        Debug.Log("CAL MIN onload: " + niftiFile.Header.cal_min);
        Debug.Log("CAL MAX onload: " + niftiFile.Header.cal_max);

        return niftiFile;
    }

    public Dictionary<Vector3Int, Voxel> ReadNiftiData(int width, int height, int depth, int niiVoxelOmissionThreshold)
    {
        if (niftiFile == null)
        {
            return null;
        }

        float calMin = niftiFile.Header.cal_min;
        float calMax = niftiFile.Header.cal_max;
        Debug.Log("CAL MIN: " + calMin);
        Debug.Log("CAL MAX: " + calMax);
        Debug.Log("AUX FILE: " + ByteToString(niftiFile.Header.aux_file));

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
                    if ((byte)niftiFile.Data[index] >= (byte)niiVoxelOmissionThreshold)
                    {                        
                        // Convert the number to a string to easily access each digit
                        // Different NII files represent colours in different ways. Decision here is to make everything in the range
                        // 0 to 254, this way greyscale will be the default but it can be turned into RGB if needed.
                        int colorGreyScale = ((int)((float)(niftiFile.Data[index] / calMax) * 254) % 254);                        
                        voxelDictionary.Add(new Vector3Int(x, y, z), new Voxel(colorGreyScale, new Vector3Int(x, y, z)));
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

    private string ByteToString(byte[] source)
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