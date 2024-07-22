using UnityEngine;

public class VisibilityThreshold : MonoBehaviour
{
    //This is the field of view that the Camera has
    float m_visibilityThreshold;

    //Set up the maximum and minimum values the Slider can return (you can change these)
    float max = 255;
    float min = 0;


    //public Camera mainCamera;
    /*    public PostProcessVolume postProcessVolume;
        private UnityEngine.Rendering.PostProcessing.DepthOfField depthOfField;*/

    void Start()
    {
        //Start the Camera field of view at 60
        m_visibilityThreshold = 1;
/*
        // Get the Depth of Field effect from the Post Process Volume
        postProcessVolume.profile.TryGetSettings(out depthOfField);*/
    }

    void Update()
    {
        //Update the camera's field of view to be the variable returning from the Slider
        WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold = (int)m_visibilityThreshold;

    }

    void OnGUI()
    {
        //This Slider changes the field of view of the Camera between the minimum and maximum values
        m_visibilityThreshold = GUI.HorizontalSlider(new Rect(1000, 40, 100, 10), m_visibilityThreshold, min, max);
        GUI.Label(new Rect(945, 35, 50, 30), "Visibilty");

    }

}