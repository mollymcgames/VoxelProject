using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FrustumCulling
{
    private Plane[] planes;
    private Bounds bounds;
    Vector3Int boundsBox;

    Vector3Int lastPosition = Vector3Int.zero;

    // Store the last camera position and rotation
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    // Thresholds for detecting significant movement or rotation
    private float positionThreshold = 0.1f;
    private float rotationThreshold = 1.0f;

    public FrustumCulling(Camera camera, int size)
    {
        lastCameraPosition = camera.transform.position;
        lastCameraRotation = camera.transform.rotation;

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

    private bool inFrustum = false;
    private float distanceToCamera = 0f;

    public bool IsVoxelInView(Camera camera, Vector3Int position, float nearClippingDistance)
    {
        inFrustum = false;
        distanceToCamera = 0f;
        if (position != lastPosition)
        {
            bounds = new Bounds(position, boundsBox);

            // Recalculate frustum planes if the camera has moved or rotated significantly
            if (HasCameraMovedOrRotated(camera))
            {
                CalculateFrustrumPlanes(camera);
                lastCameraPosition = camera.transform.position;
                lastCameraRotation = camera.transform.rotation;
            }

            // Check if the voxel is within the frustum
            inFrustum = GeometryUtility.TestPlanesAABB(planes, bounds);

            // Check if the voxel is outside the near clipping distance
            distanceToCamera = Vector3.Distance(camera.transform.position, position);

            lastPosition = position;
        }

        bool beyondNearClip = distanceToCamera > nearClippingDistance;

        return inFrustum && beyondNearClip;
    }

    private bool HasCameraMovedOrRotated(Camera camera)
    {
        // Calculate the differences in position and rotation
        float positionDifference = Vector3.Distance(camera.transform.position, lastCameraPosition);
        float rotationDifference = Quaternion.Angle(camera.transform.rotation, lastCameraRotation);

        // Check if the differences exceed the thresholds
        return positionDifference > positionThreshold || rotationDifference > rotationThreshold;
    }

}