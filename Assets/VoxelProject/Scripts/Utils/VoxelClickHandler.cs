using UnityEngine;
using UnityEngine.SceneManagement;

public class VoxelClickHandler : MonoBehaviour
{
    private float transitionDelayTime = 1.0f;
    private Animator animator;
    public string sceneToLoad;

    public Camera mainCamera;
    // The FOV value when zoomed in
    public float zoomedFOV = 90f;
    // The speed of the zoom
    public float zoomSpeed = 5f;
    private float originalFOV;

    // To check if currently zoomed in
    private bool isZoomed = false;

    private void Start()
    {
        mainCamera = Camera.main;
        originalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isZoomed = true; // !isZoomed;
            }
        }
        else
            isZoomed=false;

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse worldPosition
            Ray rayDown = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitDown;

            // Check if the ray hits the voxel
            if (Physics.Raycast(rayDown, out hitDown))
            {
                // Check if the hit object is this voxel
                if (hitDown.collider.gameObject == gameObject)
                {
                    // Load the specified scene
                    animator = GameObject.Find("Transition").GetComponent<Animator>();
                    animator.SetTrigger("TriggerTransition");
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }

        // Smoothly interpolate the FOV
        if (isZoomed)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
        }

    }
}
