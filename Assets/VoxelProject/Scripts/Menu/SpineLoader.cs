using UnityEngine;

//Class is attached to the Spine GameObject and loads the Spine file
public class SpineLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadSpineFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
