using UnityEngine;

public class HeartLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadHeartFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
