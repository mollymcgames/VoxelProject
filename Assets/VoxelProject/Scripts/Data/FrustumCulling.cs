using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FrustumCulling
{
    private Plane[] planes;
    private Bounds bounds;
    Vector3Int boundsBox;

    public FrustumCulling(Camera camera, int size)
    {
        CalculateFrustrumPlanes(camera);
        boundsBox = new Vector3Int(size, size, size);
    }
    public void CalculateFrustrumPlanes(Camera camera)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(camera);
    }

    public void DropFrustrumPlanes()
    {
        planes = null;
    }

    public bool IsChunkInView(ref Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    Vector3Int lastPosition = Vector3Int.zero;

    public bool IsVoxelInView(Camera camera, Vector3Int position, float nearClippingDistance)
    {
        bool inFrustum = false;
        float distanceToCamera = 0f;
        if (position != lastPosition)
        {
            bounds = new Bounds(position, boundsBox);
            // Check if the voxel is within the frustum
            inFrustum = GeometryUtility.TestPlanesAABB(planes, bounds);

            // Check if the voxel is outside the near clipping distance
            distanceToCamera = Vector3.Distance(camera.transform.position, position);

            lastPosition = position;
        }

        bool beyondNearClip = distanceToCamera > nearClippingDistance;

        return inFrustum && beyondNearClip;
    }
}