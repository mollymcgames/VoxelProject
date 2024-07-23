using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraFieldOfView : MonoBehaviour
{
    //This is the field of view that the Camera has
    float m_FieldOfView;
    public float sensitivity = 0.1f; // Sensitivity of mouse movement
    private bool isDragging = false;
    private float lastMouseY;

    //Set up the maximum and minimum values the Slider can return (you can change these)
    float max = 150.0f;
    float min = 20.0f;

    public Slider fovSlider;
    public TMP_Text fovValue;
    private float lastSliderValue;
    

    void Start()
    {
        //Start the Camera field of view at 60
        m_FieldOfView = 60.0f;
    }

    void Update()
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

        // Update the slider value based on mouse movement if dragging
        if (isDragging)
        {
            float mouseY = Input.mousePosition.y;
            float deltaY = lastMouseY - mouseY;
            m_FieldOfView += deltaY * sensitivity;
            m_FieldOfView = Mathf.Clamp(m_FieldOfView, min, max);
            lastMouseY = mouseY;
        }

        //Update the camera's field of view to be the variable returning from the Slider
        m_FieldOfView = fovSlider.value;
        fovValue.text = fovSlider.value.ToString();
        Camera.main.fieldOfView = m_FieldOfView;
    }

    /*    void OnGUI()
        {
            //This Slider changes the field of view of the Camera between the minimum and maximum values
            m_FieldOfView = GUI.HorizontalSlider(new Rect(1000, 20, 100, 10), m_FieldOfView, min, max);
            GUI.Label(new Rect(970, 15, 50, 30), "FoV");
        }*/

}