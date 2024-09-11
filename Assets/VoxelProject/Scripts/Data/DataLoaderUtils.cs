using UnityEngine;

// Returns the appropriate data loader based on the file type.
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

}