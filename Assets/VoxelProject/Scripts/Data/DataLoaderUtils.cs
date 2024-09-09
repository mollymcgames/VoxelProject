using UnityEngine;

public class DataLoaderUtils
{
    public static ASourceDataLoader LoadDataFile(int voxelOmissionThreshold=0)
    {
        if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath.Contains(".nii"))
            return new SourceDataLoader(voxelOmissionThreshold, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
        else if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath.Contains(".txt"))
            return new SourceDataTextFileLoader(WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize);
        else
        {
            Debug.LogError("Unknown file type, can't go on!");
            return null;
        }
    }

    public static string GetDataFileFormat()
    {
        if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath.Contains(".nii"))
            return "nii";
        else if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath.Contains(".txt"))
            return "txt";
        else
        {
            Debug.LogError("Unknown file type, can't go on!");
            return null;
        }
    }


}