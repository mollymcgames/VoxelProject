using UnityEngine;

public class GeneralMethods
{
    private const float maxDistance = 30f;
    private const float minDistance = 10f;

    public static float AdjustMaterialTransparency(ref VoxelContainer container)
    {
        float distance = Vector3.Distance(Camera.main.transform.position, container.transform.position);

        // Adjust the transparency of the material based on the camera distance
        // alpha should be 0 if close and 1 if far
        float alpha = Mathf.InverseLerp(minDistance, maxDistance, distance);

        /*        if (container != null)
                {
                    //Debug.Log("Adjusting transparency with alpha: " + alpha);
                }
                else
                {
                    Debug.LogWarning("No Renderer found on the container.");
                }
        */
        return alpha;
    }
}