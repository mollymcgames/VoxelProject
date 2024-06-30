using UnityEngine;

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
        Camera.main.fieldOfView = m_FieldOfView;

/*        // Adjust the blur intensity based on the camera's FOV
        float fov = m_FieldOfView; // mainCamera.fieldOfView;

        // Example: Increase blur as FOV decreases
        float blurIntensity = Mathf.Lerp(10f, 0f, (fov - 30f) / (90f - 30f));
        depthOfField.focalLength.value = blurIntensity;*/

    }

    void OnGUI()
    {
        //This Slider changes the field of view of the Camera between the minimum and maximum values
        m_FieldOfView = GUI.HorizontalSlider(new Rect(20, 20, 100, 40), m_FieldOfView, min, max);
    }

}