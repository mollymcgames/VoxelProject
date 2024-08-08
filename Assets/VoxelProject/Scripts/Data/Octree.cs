using UnityEngine;

public class Octree
{
    public OctreeNode root;

    public Octree(Vector3Int rootPosition, int rootSize)
    {
        root = new OctreeNode(rootPosition, rootSize);
    }

    public void Insert(Vector3Int position, object voxelData)
    {
        Insert(root, position, voxelData);
    }

    private void Insert(OctreeNode node, Vector3Int position, object voxelData, int depth = 0, int maxDepth = 10)
    {
        if (depth >= maxDepth)
        {
            // Treat this node as a leaf if max depth is reached
            node.Voxel = voxelData;
            return;
        }

        if (node.IsLeaf())
        {
            if (node.Voxel == null)
            {
                node.Voxel = voxelData;
                return;
            }
            else
            {
                Subdivide(node); // Subdivide only once if needed
                Insert(node, node.Position, node.Voxel, depth + 1, maxDepth); // Reinsert existing voxel
                node.Voxel = null;
                //Insert(node, position, voxelData);
            }
        }

        // Insert into the appropriate child node
        int childIndex = GetChildIndex(node, position);
        if (node.Children[childIndex] == null)
        {
            Vector3Int childPosition = GetChildPosition(node, childIndex);
            int childSize = node.Size / 2;
            node.Children[childIndex] = new OctreeNode(childPosition, childSize);
        }
        Insert(node.Children[childIndex], position, voxelData, depth + 1, maxDepth); // Increase depth


/*        else
        {
            int childIndex = GetChildIndex(node, position);
            if (node.Children[childIndex] == null)
            {
                Vector3 childPosition = GetChildPosition(node, childIndex);
                float childSize = node.Size / 2;
                node.Children[childIndex] = new OctreeNode(childPosition, childSize);
            }
            Insert(node.Children[childIndex], position, voxelData);
        }*/
    }

    private void Subdivide(OctreeNode node)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3Int childPosition = GetChildPosition(node, i);
            int childSize = node.Size / 2;
            node.Children[i] = new OctreeNode(childPosition, childSize);
        }
    }

    private int GetChildIndex(OctreeNode node, Vector3Int position)
    {
        int index = 0;
        if (position.x >= node.Position.x) index |= 1;
        if (position.y >= node.Position.y) index |= 2;
        if (position.z >= node.Position.z) index |= 4;
        return index;
    }

    private Vector3Int GetChildPosition(OctreeNode node, int index)
    {
        int offset = node.Size / 4;
        return new Vector3Int(
            node.Position.x + ((index & 1) == 0 ? -offset : offset),
            node.Position.y + ((index & 2) == 0 ? -offset : offset),
            node.Position.z + ((index & 4) == 0 ? -offset : offset)
        );
    }
}
