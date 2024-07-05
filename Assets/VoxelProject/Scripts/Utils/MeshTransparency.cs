using UnityEngine;

public class MeshTransparency : MonoBehaviour
{
    public GameObject mesh;
    public float maxDistance = 30f;
    public float minDistance = 10f;

    void Update()
    {
        AdjustMaterialTransparency();
    }

    public void AdjustMaterialTransparency()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, mesh.transform.position);
        
        // Adjust the transparency of the material based on the camera distance
        // alpha should be 0 if close and 1 if far
        float alpha = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // Get the material of the mesh
        Renderer meshRenderer = mesh.GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            Material material = meshRenderer.material;

            // Assuming the material has a color property with alpha
            Color color = material.color;
            color.a = alpha;
            material.color = color;

             // Log the details
            // Debug.Log($"Distance to mesh: {distance}");
            // Debug.Log($"Calculated alpha: {alpha}");
            // Debug.Log($"Material color: {material.color}");            

            // If using a shader with a transparency property, you might need to set that too
            // material.SetFloat("_Transparency", alpha);
        }
    }
}
