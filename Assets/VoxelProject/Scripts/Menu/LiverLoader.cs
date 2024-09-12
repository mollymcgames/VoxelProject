using UnityEngine;

//Class is attached to the Liver GameObject and loads the Liver file
public class LiverLoader : MonoBehaviour
{
    public CustomCursorHandler cch;

    void OnMouseDown() {
        cch.RestoreDefaultCursor();
        WorldManager.Instance.menuHandler.LoadLiverFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
