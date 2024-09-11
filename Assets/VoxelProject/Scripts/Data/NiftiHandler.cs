using System.Collections.Generic;
using Nifti.NET;
using UnityEngine;

// This is an important class to handle the Nifti file. It is used to read the Nifti file and convert it into a dictionary of voxels.
public class NiftiHandler: ScriptableObject
{
    private Nifti.NET.Nifti niftiFile = null; //Stores the Nifti file

    // Reads the Nifti file 
    public Nifti.NET.Nifti ReadNiftiFileOnly(string niftiFilePath)
    {
        //Check if the file path is valid and the file is not already loaded
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
        //calculate the calibrated min and max values of the data
        float calMin = niftiFile.Header.cal_min;
        float calMax = niftiFile.Header.cal_max;
        if (calMax <= 0)
        {
            int index = 0;
            //Reason why loading is slower is due to going through the entire file to find the min and max cal values,
            // to calculate the greyscale value of each voxel.
            foreach (var item in niftiFile.Data)
            {
                if (niftiFile.Data[index] < calMin)
                    calMin = (float)niftiFile.Data[index];

                if (niftiFile.Data[index] > calMax)
                    calMax = (float)niftiFile.Data[index];

                index++;
            }
        }
        //overwrite the cal min and max values from worked out values
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
                        // Scale the voxel intensity to a 0-254 range (greyscale) and store it in the voxel
                        int colorGreyScale = (int)(niftiFile.Data[index] / calMax * 254) % 254;                        
                        voxelDictionary.Add(new Vector3Int(x, y, z), new Voxel(colorGreyScale, new Vector3Int(x, y, z)));
                        voxelsLoaded++; // Increment the number of voxels loaded to keep track of the voxels loaded
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

    //Convert byte array to string to show the file name
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