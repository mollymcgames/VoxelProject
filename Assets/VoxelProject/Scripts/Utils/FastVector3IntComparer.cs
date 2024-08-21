using System.Collections.Generic;
using UnityEngine;

/**
 * This class implements the EqualityComparer interface and is used to help make 
 * a Dictionary with a Vector3Int key perform faster.
 */
public class FastVector3IntComparer : IEqualityComparer<Vector3Int>
{
    public bool Equals(Vector3Int a, Vector3Int b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public int GetHashCode(Vector3Int obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + obj.x;
            hash = hash * 31 + obj.y;
            hash = hash * 31 + obj.z;
            return hash;
        }
    }
}
