using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.PostProcessing;

public class CameraFieldOfView : MonoBehaviour
{
    //This is the field of view that the Camera has
    float m_FieldOfView;
    //public Camera mainCamera;
/*    public PostProcessVolume postProcessVolume;
    private UnityEngine.Rendering.PostProcessing.DepthOfField depthOfField;*/

    void Start()
    {
        //Start the Camera field of view at 60
        m_FieldOfView = 60.0f;
/*
        // Get the Depth of Field effect from the Post Process Volume
        postProcessVolume.profile.TryGetSettings(out depthOfField);*/


    }

    void Update()
    {
        //Update the camera's field of view to be the variable returning from the Slider
        Camera.main.fieldOfView = m_FieldOfView;

/*        // Adjust the blur intensity based on the camera's FOV
        float fov = m_FieldOfView; // mainCamera.fieldOfView;

        // Example: Increase blur as FOV decreases
        float blurIntensity = Mathf.Lerp(10f, 0f, (fov - 30f) / (90f - 30f));
        depthOfField.focalLength.value = blurIntensity;*/

    }

    void OnGUI()
    {
        //Set up the maximum and minimum values the Slider can return (you can change these)
        float max, min;
        max = 150.0f;
        min = 20.0f;
        //This Slider changes the field of view of the Camera between the minimum and maximum values
        m_FieldOfView = GUI.HorizontalSlider(new Rect(20, 20, 100, 40), m_FieldOfView, min, max);
    }

}