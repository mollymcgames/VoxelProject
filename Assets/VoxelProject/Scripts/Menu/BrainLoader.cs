using UnityEngine;

public class BrainLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadBrainFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
