using UnityEngine;

public class OctreeNode
{
    public Vector3Int Position { get; private set; }
    public int Size { get; private set; }
    public OctreeNode[] Children { get; private set; }
    public object Voxel { get; set; }

    public OctreeNode(Vector3Int position, int size)
    {
        Position = position;
        Size = size;
        Children = new OctreeNode[8];
        Voxel = null;
    }

    public bool IsLeaf()
    {
        foreach (var child in Children)
        {
            if (child != null)
                return false;
        }
        return true;
    }
}