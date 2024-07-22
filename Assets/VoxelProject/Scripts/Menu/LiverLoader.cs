using UnityEngine;

public class LiverLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadLiverData();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
