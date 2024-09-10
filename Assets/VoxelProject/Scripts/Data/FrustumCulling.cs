using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FrustumCulling
{
    private Plane[] planes; //Array of planes representing the camera frustum
    private Bounds bounds; //Bounds representing the voxel chunk
    Vector3Int boundsBox; //Size of the voxel chunk

    Vector3Int lastPosition = Vector3Int.zero; //Last position of the voxel chunk checked to prevent redundant checks

    // Store the last camera position and rotation
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    // Thresholds for detecting significant movement or rotation
    private float positionThreshold = 0.1f; // Position threshold
    private float rotationThreshold = 1.0f; //Rotation threshold

    public FrustumCulling(Camera camera, int size)
    {
        // Store the initial camera position and rotation
        lastCameraPosition = camera.transform.position;
        lastCameraRotation = camera.transform.rotation;

        // Calculate the initial frustum planes for the camera
        CalculateFrustrumPlanes(camera);
        boundsBox = new Vector3Int(size, size, size);
    }
    public void CalculateFrustrumPlanes(Camera camera)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(camera);
    }

    public void DropFrustrumPlanes()
    {
        planes = null; //Clear the planes array when not in use
    }

    public bool IsChunkInView(ref Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    private bool inFrustum = false;
    private float distanceToCamera = 0f;

    public bool IsVoxelInView(Camera camera, Vector3Int position, float nearClippingDistance)
    {
        inFrustum = false; //Reset the inFrustum flag
        distanceToCamera = 0f; //Reset the distance to camera

        //Chek if the position has changed since the last frame
        if (position != lastPosition)
        {
            bounds = new Bounds(position, boundsBox); //Set the bounds to the new position

            // Recalculate frustum planes if the camera has moved or rotated significantly
            if (HasCameraMovedOrRotated(camera))
            {
                CalculateFrustrumPlanes(camera);
                lastCameraPosition = camera.transform.position;
                lastCameraRotation = camera.transform.rotation;
            }

            // Check if the voxel is within the frustum
            inFrustum = GeometryUtility.TestPlanesAABB(planes, bounds);

            // Calculate the distance from the camera to the voxel
            distanceToCamera = Vector3.Distance(camera.transform.position, position);

            lastPosition = position; //Store the current position as the last position
        }

        bool beyondNearClip = distanceToCamera > nearClippingDistance;

        return inFrustum && beyondNearClip;
    }

    //Check if the camera has moved or rotated significantly
    private bool HasCameraMovedOrRotated(Camera camera)
    {
        // Calculate the differences in position and rotation
        float positionDifference = Vector3.Distance(camera.transform.position, lastCameraPosition);
        float rotationDifference = Quaternion.Angle(camera.transform.rotation, lastCameraRotation);

        // Check if the movement or rotation exceeds the predefined thresholds
        return positionDifference > positionThreshold || rotationDifference > rotationThreshold;
    }

}