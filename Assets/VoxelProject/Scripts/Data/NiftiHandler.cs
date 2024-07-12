using System;
using System.IO;
using Nifti.NET;
using Unity.VisualScripting;
using UnityEngine;

public class NiftiHandler : MonoBehaviour
{      
    public static int visibleColourThreshold = 5;

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
        float calMin = niftiData.Header.cal_min;
        float calMax = niftiData.Header.cal_max;
        Debug.Log("CAL MIN: " + calMin);
        Debug.Log("CAL MAX: " + calMax);
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
                    //Debug.Log("Next RAW vox colour=" + (float)niftiData.Data[index]); 
                    //Debug.Log("Next vox colour=" + (int)(niftiData.Data[index]/calMax * 254 )); 
                    // Convert the number to a string to easily access each digit                   
                    voxelValue[x, y, z] = new Voxel( (float)niftiData.Data[index] > visibleColourThreshold, calMax <= 255, calMax <= 255 ? (int) (niftiData.Data[index]/calMax * 254) : MakeIntColourNumber(niftiData.Data[index].ToString()) );
                    index++;
                }
            }
        }
        return voxelValue;
    }

    private static Int32 MakeIntColourNumber(string numberStr)
    {
        int r = 9;
        int g = 9;
        int b = 9;

        // Extract each digit and convert to integer 
        // Scale 0-9 to 0-252 (0-255)
        try { r = int.Parse(numberStr[0].ToString()) * 28; } catch { } 
        try { g = int.Parse(numberStr[1].ToString()) * 28;} catch { }
        try { b = int.Parse(numberStr[2].ToString()) * 28; } catch { }
        return Convert.ToInt32($"#{r:X2}{g:X2}{b:X2}".Replace("#", ""), 16);
    }

    public static Voxel[,,] ReadNiftiSegmentData(ref Voxel[,,] voxelData, int segmentLayer, Nifti.NET.Nifti niftiSegmentFileLines, int widthX, int heightY, int depthZ)
    {
        float calMin = niftiSegmentFileLines.Header.cal_min;
        float calMax = niftiSegmentFileLines.Header.cal_max;
        Debug.Log("CAL MIN SEGMENT: " + calMin);
        Debug.Log("CAL MAX SEGMENT: " + calMax);

        int numVoxels = widthX * heightY * depthZ;

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depthZ; z++)
        {
            for (int y = 0; y < heightY; y++)
            {
                for (int x = 0; x < widthX; x++)
                {
//                    voxelData[x, y, z] = new Voxel((float)niftiSegmentFileLines.Data[index] > visibleColourThreshold, calMax <= 255, calMax <= 255 ? (int)(niftiSegmentFileLines.Data[index] / calMax * 254) : MakeIntColourNumber(niftiSegmentFileLines.Data[index].ToString()));
                    if ( (int)niftiSegmentFileLines.Data[index] > 0 )
                        voxelData[x, y, z].addColourRGBLayer(segmentLayer, calMax <= 255 ? (int)(niftiSegmentFileLines.Data[index] / calMax * 254) : MakeIntColourNumber((niftiSegmentFileLines.Data[index]/2).ToString()));
                    index++;
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