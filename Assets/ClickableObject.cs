using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableObject : MonoBehaviour
{
    public string sceneToLoad;

    void Start()
    {
        // Ensure the object has a collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"{gameObject.name} clicked!");
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name not set. Please set the sceneToLoad variable.");
        }
    }
}
