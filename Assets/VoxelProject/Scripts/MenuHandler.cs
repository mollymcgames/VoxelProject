using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("World");
    }

    public void LoadHeartFile()
    {
        string niftiFilePath = "Assets/Resources/JHU-WhiteMatter-labels-2mm.nii"; 
        SourceDataLoader.OpenNiftiFile(niftiFilePath);
        Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();
        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);
    }

    public void LoadLRFile()
    {
        string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii"; 
        SourceDataLoader.OpenNiftiFile(niftiFilePath);
        Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();

        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);
    }
}
