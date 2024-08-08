using System.Collections.Generic;
using UnityEngine;

public class OctreeTraversal
{
    public static List<Vector3Int> GetVisibleVoxels(OctreeNode node, Camera camera)
    {
        List<Vector3Int> visibleVoxels = new List<Vector3Int>();
        Traverse(node, camera, visibleVoxels);
        return visibleVoxels;
    }

    private static void Traverse(OctreeNode node, Camera camera, List<Vector3Int> visibleVoxels)
    {
        if (node == null)
            return;

        if (FrustumCulling.IsVoxelInView(camera, node.Position, node.Size))
        {
            if (node.IsLeaf())
            {
                if (node.Voxel != null)
                {
                    visibleVoxels.Add(node.Position);
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    Traverse(child, camera, visibleVoxels);
                }
            }
        }
    }
}
