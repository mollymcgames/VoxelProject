using UnityEngine;

public class FrustumCulling
{
    private static Plane[] planes;
    private static Bounds bounds;

    public static bool IsVoxelInView(Camera camera, Vector3Int position, int size, float nearClippingDistance)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(camera);
        bounds = new Bounds(position, new Vector3Int(size, size, size));

        // Check if the voxel is within the frustum
        bool inFrustum = GeometryUtility.TestPlanesAABB(planes, bounds);

        // Check if the voxel is outside the near clipping distance
        float distanceToCamera = Vector3.Distance(camera.transform.position, position);
        bool beyondNearClip = distanceToCamera > nearClippingDistance;

        return inFrustum && beyondNearClip;
    }
}