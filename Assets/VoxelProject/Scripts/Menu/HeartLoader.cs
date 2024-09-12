using UnityEngine;

//Class is attached to the Heart GameObject and loads the Heart file
public class HeartLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadHeartFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
