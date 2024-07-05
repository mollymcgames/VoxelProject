using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLoader : MonoBehaviour
{
    void OnMouseDown() {
        MenuHandler menuHandler = new MenuHandler();
        menuHandler.LoadHeartFile();
        menuHandler.LoadNextScene();
    }
}
