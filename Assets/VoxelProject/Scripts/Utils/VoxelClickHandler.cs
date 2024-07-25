using UnityEngine;
using UnityEngine.SceneManagement;

public class VoxelClickHandler : MonoBehaviour
{
    private float transitionDelayTime = 1.0f;
    private Animator animator;
    public string sceneToLoad;

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse worldPosition
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits the voxel
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is this voxel
                if (hit.collider.gameObject == gameObject)
                {
                    // Load the specified scene
                    animator = GameObject.Find("Transition").GetComponent<Animator>();
                    animator.SetTrigger("TriggerTransition");
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}
