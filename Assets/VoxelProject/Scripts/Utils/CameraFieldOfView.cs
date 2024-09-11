using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Manages the field of view of the Camera in the world scene 
public class CameraFieldOfView : MonoBehaviour
{
    float m_FieldOfView = 60.0f; //The current field of view that the camera has 
    public float sensitivity = 0.1f; // Sensitivity of mouse movement
    private bool isDragging = false;
    private float lastMouseY;

    // Set up the maximum and minimum values the Slider can return
    float max = 150.0f;
    float min = 20.0f;

    public Slider fovSlider;
    public TMP_Text fovValue;    


    void Update()
    {
        //Checks if zooming is allowed
        if (SCManager.Instance.isZooming == false)
        {
            // Check if the mouse wheel button is pressed
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMouseY = Input.mousePosition.y;
            }

            // Check if the mouse wheel button is released
            if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }

            // Update the slider color based on mouse movement if dragging
            if (isDragging)
            {
                float mouseY = Input.mousePosition.y;
                float deltaY = lastMouseY - mouseY;
                m_FieldOfView += deltaY * sensitivity;
                m_FieldOfView = Mathf.Clamp(m_FieldOfView, min, max);
                lastMouseY = mouseY; //Update the last Y position for the next frame
            }

            // Apply the calculated FOV to the camera only if it has changed
            if (Camera.main.fieldOfView != m_FieldOfView)
            {
                Camera.main.fieldOfView = m_FieldOfView;
            }

            // Update the camera's field of view to be the variable returning from the Slider
            m_FieldOfView = fovSlider.value;
            fovValue.text = ((int)fovSlider.value).ToString();
            Camera.main.fieldOfView = m_FieldOfView;
        }
    }

}