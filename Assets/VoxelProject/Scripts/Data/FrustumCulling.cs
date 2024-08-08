using UnityEngine;

public class FrustumCulling
{
    public static bool IsVoxelInView(Camera camera, Vector3Int position, int size)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Bounds bounds = new Bounds(position, new Vector3Int(size, size, size));
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}