using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public static Vector3 containerPosition = Vector3.zero; //Static field to store the container position
    public void LoadNextScene()
    {
        SceneManager.LoadScene("World");
    }

    public void LoadHeartFile()
    {
        containerPosition = new Vector3(0, -55, 0); //Set the position of the container
        //Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, "JHU-WhiteMatter-labels-2mm.nii");
        // string niftiFilePath = "Assets/Resources/JHU-WhiteMatter-labels-2mm.nii"; 
        SourceDataLoader.OpenNiftiFile(niftiFilePath);
        Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();
        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);
    }

    public void LoadLRFile()
    {
        containerPosition = new Vector3(10, -50, 130); //Set the position of the container
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, "avg152T1_LR_nifti.nii");
        // string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii"; 
        SourceDataLoader.OpenNiftiFile(niftiFilePath);
        Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();

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
