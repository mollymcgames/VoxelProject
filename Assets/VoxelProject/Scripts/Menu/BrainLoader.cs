using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainLoader : MonoBehaviour
{
    void OnMouseDown() {
        Debug.Log("BrainLoader:OnMouseDown");
        MenuHandler menuHandler = new MenuHandler();
        menuHandler.LoadLRFile();
        menuHandler.LoadNextScene();
    }
}
