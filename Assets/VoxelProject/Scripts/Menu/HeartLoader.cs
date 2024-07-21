using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLoader : MonoBehaviour
{
    void OnMouseDown() {
        WorldManager.Instance.menuHandler.LoadHeartFile();
        WorldManager.Instance.menuHandler.LoadNextScene();
    }
}
