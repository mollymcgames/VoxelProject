using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Camera playerCamera;
    public float transitionDistance = 5.0f;
    public float zoomSpeed = 0.1f;
    public CanvasGroup fadeGroup;

    private bool isTransitioning = false;
    private float currentZoomLevel = 1.0f;
    private float targetZoomLevel = 4.0f;

    void Update()
    {
//        if (Vector3.Distance(playerCamera.transform.position, redCube.position) < transitionDistance ||
 //           Vector3.Distance(playerCamera.transform.position, greenCube.position) < transitionDistance)
        if (VoxelWorldManager.Instance.doSceneSwitch)
        {
            StartCoroutine(ZoomAndTransition());
        }
    }

    IEnumerator ZoomAndTransition()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Zoom in
        while (currentZoomLevel < targetZoomLevel)
        {
            playerCamera.fieldOfView -= zoomSpeed;
            currentZoomLevel += zoomSpeed;
            yield return null;
        }

        // Fade out
        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime;
            yield return null;
        }

        // Load second scene
        SceneManager.LoadScene(VoxelWorldManager.Instance.sceneSwitchName);

        // Fade in
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
    }
}