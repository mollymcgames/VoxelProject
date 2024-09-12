using UnityEngine;

//Class is attached to the Brain GameObject and loads the brain file
public class BrainLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadBrainFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
