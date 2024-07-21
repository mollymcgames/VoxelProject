using Nifti.NET;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataLoader : ASourceDataLoader
{
    public SourceDataLoader(int chunkSize) : base(chunkSize) { }

    private static Nifti.NET.Nifti niftiFileLines = null;
    private static Nifti.NET.Nifti niftiSegmentFileLines = null;    
    
    public override VoxelStruct[,,] LoadSourceData(string filepath)
    {
/*        Debug.Log("Loading nii source data...:" + filepath);
        LoadNiftiFile(filepath);
        CreateVoxelsArray();*/
        return voxelData;
    }

    public override VoxelGrid LoadSourceDataGrid(string filepath)
    {
        Debug.Log("Loading nii source data in GRID format...:" + filepath);
        LoadNiftiFile(filepath);
        CreateVoxelsArrayGrid();
        return voxelGrid;
    }

    public override VoxelStruct[,,] LoadSegmentData(ref VoxelStruct[,,] sourceData, int segmentLayer, string segmentFile)
    {
        Debug.Log("Loading nii segment data..." + segmentFile + " into layer: "+segmentLayer);
        LoadNiftiSegmentFile(Path.Combine(Application.streamingAssetsPath, segmentFile));
        AddSegmentToVoxelsArray(segmentLayer);
        return voxelData;
    }

    public override object GetHeader()
    {
        return niftiFileLines;
    }

    #region
    //private Dictionary<Vector3Int, Chunk> LoadNiftiFile(string niftiFilePath)
    private void LoadNiftiFile(string filePath)
    {
        // Load default file
        niftiFileLines = ReadNiftiFile(filePath);

        // Get the dimensions
        widthX = niftiFileLines.Dimensions[0];
        heightY = niftiFileLines.Dimensions[1];
        depthZ = niftiFileLines.Dimensions[2];

        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);
    }

    private void LoadNiftiSegmentFile(string filePath)
    {
        // Load default file
        niftiSegmentFileLines = ReadNiftiFile(filePath);

        // Get the dimensions
        widthX = niftiSegmentFileLines.Dimensions[0];
        heightY = niftiSegmentFileLines.Dimensions[1];
        depthZ = niftiSegmentFileLines.Dimensions[2];
    }

    private void CreateVoxelsArray()
    {
        // Read the voxel data
        voxelData = NiftiHandler.ReadNiftiData(niftiFileLines, widthX, heightY, depthZ);
        Debug.Log("Data now read in");
    }

    private void CreateVoxelsArrayGrid()
    {
        // Read the voxel data
        voxelGrid = NiftiHandler.ReadNiftiDataGrid(niftiFileLines, widthX, heightY, depthZ, chunkSize);
        Debug.Log("Data (grid) now read in");
    }

    private void AddSegmentToVoxelsArray(int segmentLayer)
    {
        // Read the voxel data
        voxelData = NiftiHandler.ReadNiftiSegmentData(ref voxelData, segmentLayer, niftiSegmentFileLines, widthX, heightY, depthZ);
        Debug.Log("Segment data now read in");
    }

    public void OpenNiftiFile(string filePath)
    {
        niftiFileLines = ReadNiftiFile(filePath);
    }

    private Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiHandler.ReadNiftiFile(niftiFilePath);
    }


    /*    private List<VoxelElement> ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
        {
            List<VoxelElement> voxelDataList = new List<VoxelElement>();

            // Iterate through each voxel
            int index = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x > maxX) maxX = x;
                        if (x < minX) minX = x;
                        if (y > maxY) maxY = y;
                        if (y < minY) minY = y;
                        if (z > maxZ) maxZ = z;
                        if (z < minZ) minZ = z;

                        // Assign the parsed color value as the voxel value if it's going to be visible
                        if (niftiData.Data[index] > 10)
                            voxelDataList.Add(new VoxelElement(new Vector3Int(x, y, z), niftiData.Data[index].ToString()));
                        index++;
                    }
                }
            }
            Debug.Log("Lines processed:" + index);
            return voxelDataList;
        }*/

    #endregion

}