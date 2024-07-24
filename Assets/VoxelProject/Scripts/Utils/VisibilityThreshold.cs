using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisibilityThreshold : MonoBehaviour
{
    //This is the field of view that the Camera has
    //float m_visibilityThreshold;

    //Set up the maximum and minimum values the Slider can return (you can change these)
    float max = 0;
    float min = 254;

    public Slider visibilityThresholdSlider;
    private float lastSliderValue;
    public TMP_Text visibilityThresholdValue;
    private bool sliderChanged;
    private Coroutine delayedRenderCoroutine;

    void Start()
    {
        //Start the Camera field of view at 60
        // m_visibilityThreshold = 1;

        if (visibilityThresholdSlider != null)
        {
            visibilityThresholdSlider.onValueChanged.AddListener(OnSliderValueChanged);
            lastSliderValue = visibilityThresholdSlider.value;
        }
    }

    void OnSliderValueChanged(float value)
    {
        sliderChanged = true;

        // If there's an ongoing coroutine, stop it
        if (delayedRenderCoroutine != null)
        {
            StopCoroutine(delayedRenderCoroutine);
        }

        // Start the coroutine to wait before re-rendering the mesh
        delayedRenderCoroutine = StartCoroutine(DelayedRender());
    }

    IEnumerator DelayedRender()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        if (sliderChanged && lastSliderValue != visibilityThresholdSlider.value)
        {
            // Update the mesh only if the slider color has changed
            lastSliderValue = visibilityThresholdSlider.value;
            sliderChanged = false;
            ComputeManager.Instance.GenerateVoxelData(ref SCManager.Instance.container, 0);
            //Debug.Log("Would render now...");
        }
    }

    void Update()
    {
        visibilityThresholdValue.text = visibilityThresholdSlider.value.ToString();

        //Update the camera's field of view to be the variable returning from the Slider
        WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold = (int)lastSliderValue; // (int)m_visibilityThreshold;
    }

/*    void OnGUI()
    {
        //This Slider changes the field of view of the Camera between the minimum and maximum values
        m_visibilityThreshold = GUI.HorizontalSlider(new Rect(1000, 40, 100, 10), m_visibilityThreshold, min, max);
        GUI.Label(new Rect(945, 35, 50, 30), "Visibilty");
    }*/

}