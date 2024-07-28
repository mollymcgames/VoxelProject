using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoxelClickHandler : MonoBehaviour
{
    private float transitionDelayTime = 1.0f;
    private Animator animator;
    public string sceneToLoad;

    public bool zoomLock = false;

    // The FOV value when zoomed in
    public float zoomedFOV = 20f;
    // The speed of the zoom
    public float zoomInSpeed = 2f;
    public float zoomOutSpeed = 1.01f;

    private float originalFOV;

    // To check if currently zoomed in
    private bool isZoomedOut = false;

    private void Start()
    {
        originalFOV = Camera.main.fieldOfView;
    }

    void OnMouseExit()
    {
        if (gameObject.CompareTag("SegmentOne"))
        {
/*            if (SCManager.Instance.isZooming == true)
            {*/
                Debug.Log("Mouse out! TD=" + Time.deltaTime);
                isZoomedOut = false;
                SCManager.Instance.isZooming = false;
                //zoomLock = false;
            //}
        }
    }

    void CheckForHover()
    {
        if (SCManager.Instance.isZooming == false)
        { 
            // Raycast from the mouse worldPosition
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object hit has the specified tag
                //if (hit.collider.gameObject == gameObject)
                if (hit.collider.CompareTag("SegmentOne"))
                {
                    Debug.Log("Collided into SegmentOne. ZL="+ SCManager.Instance.isZooming);
                    zoomLock = true;
                    isZoomedOut = true; // !isZoomedOut;
                    SCManager.Instance.isZooming = true;
                }
            }
        }
    }

    void Update()
    {
        CheckForHover();

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
            {
                // Create a ray from the camera to the mouse worldPosition
                Ray rayDown = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        if (SCManager.Instance.isZooming)
        {
            // Smoothly interpolate the FOV
            if (isZoomedOut)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomedFOV, Time.deltaTime * zoomInSpeed);
                if (Camera.main.fieldOfView <= zoomedFOV)
                {
                    SCManager.Instance.isZooming = false;
                    isZoomedOut = false;
                }
            }
            else
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV, Time.deltaTime * zoomOutSpeed);
                if (Camera.main.fieldOfView >= originalFOV)
                {
                    SCManager.Instance.isZooming = true;
                    //isZoomedOut = true;
                }
            }
        }
    }
}
