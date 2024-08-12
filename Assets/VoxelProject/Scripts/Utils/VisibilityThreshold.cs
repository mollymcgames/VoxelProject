using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisibilityThreshold : MonoBehaviour
{
    //Set up the maximum and minimum values the Slider can return (you can change these)
    float max = 0;
    float min = 254;

    public Slider visibilityThresholdSlider;
    public Toggle invertVisibility;
    private float lastSliderValue;
    public TMP_Text visibilityThresholdValue;
    private bool sliderChanged;
    private Coroutine delayedRenderCoroutine;

    void Start()
    {
        if (visibilityThresholdSlider != null)
        {
            visibilityThresholdSlider.onValueChanged.AddListener(OnSliderValueChanged);
            lastSliderValue = visibilityThresholdSlider.value;
        }

        if (invertVisibility != null)
        {
            invertVisibility.onValueChanged.AddListener(OnToggleChanged);
        }

    }

    void OnToggleChanged(bool change)
    {
        callCoRoutine();
    }

    void OnSliderValueChanged(float value)
    {
        callCoRoutine();
    }

    private void callCoRoutine()
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
        }
    }

    void Update()
    {
        visibilityThresholdValue.text = visibilityThresholdSlider.value.ToString();

        if (invertVisibility.isOn)
        {
            //Update the camera's field of view to be the variable returning from the Slider        
            WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold = 254 - (int)lastSliderValue; // (int)m_visibilityThreshold;
        }
        else
        {
            //Update the camera's field of view to be the variable returning from the Slider        
            WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold = (int)lastSliderValue; // (int)m_visibilityThreshold;
        }
    }

}