using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverLoader : MonoBehaviour
{
    void OnMouseDown() {
        MenuHandler menuHandler = new MenuHandler();
        menuHandler.LoadSegmentData();
        menuHandler.LoadNextScene();
    }
}
