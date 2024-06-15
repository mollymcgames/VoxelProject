using UnityEngine;

public class SetChildMaterials : MonoBehaviour
{
    public Material newMaterial; // Assign this in the Inspector

    void Start()
    {
        // Get all MeshRenderer components in the children of this GameObject
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();

        // Loop through each MeshRenderer and set its material
        foreach (MeshRenderer renderer in childRenderers)
        {
            renderer.material = newMaterial;
        }
    }
}