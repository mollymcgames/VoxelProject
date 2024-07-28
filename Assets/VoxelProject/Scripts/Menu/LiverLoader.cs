using UnityEngine;

public class LiverLoader : MonoBehaviour
{
    public CustomCursorHandler cch;

    void OnMouseDown() {
        cch.RestoreDefaultCursor();
        WorldManager.Instance.menuHandler.LoadLiverData();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
