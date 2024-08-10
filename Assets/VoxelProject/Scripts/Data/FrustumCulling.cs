using UnityEngine;

public class FrustumCulling
{
    private static Plane[] planes;
    private static Bounds bounds;

    public static bool IsVoxelInView(Camera camera, Vector3Int position, int size)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(camera);
        bounds = new Bounds(position, new Vector3Int(size, size, size));
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}