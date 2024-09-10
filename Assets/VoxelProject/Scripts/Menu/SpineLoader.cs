using UnityEngine;

public class SpineLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadSpineFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
