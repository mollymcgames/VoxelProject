using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
/*    VoxelWorldManager vwm;
    
    public void Start()
    {
        vwm = new VoxelWorldManager();
    }*/

    public void LoadNextScene()
    {
        SceneManager.LoadScene("World");
    }

    public void LoadHeartFile()
    {
        VoxelWorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        LoadAFile();
    }

    public void LoadLRFile()
    {
        VoxelWorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        LoadAFile();
    }

    private void LoadAFile()
    {
        //Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, VoxelWorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        ASourceDataLoader loader = DataLoaderUtils.LoadDataFile();
        loader.LoadSourceData(VoxelWorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);

        //SourceDataLoader.OpenNiftiFile(niftiFilePath);
        Nifti.NET.Nifti niftiFile = (Nifti.NET.Nifti)loader.GetHeader(); // SourceDataLoader.GetHeader();

        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);
    }

    public void LoadZoom()
    {
        SceneManager.LoadScene("Zooming");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
