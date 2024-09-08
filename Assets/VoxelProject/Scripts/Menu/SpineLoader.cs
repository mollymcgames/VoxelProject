using UnityEngine;

public class SpineLoader : MonoBehaviour
{
    public CustomCursorHandler cch;

    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadSpineFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
