using UnityEngine;
using UnityEngine.SceneManagement;

public class VoxelClickHandler : MonoBehaviour
{
    // Name of the scene to load
    public string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits the voxel
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is this voxel
                if (hit.collider.gameObject == gameObject)
                {
                    // Load the specified scene
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}
