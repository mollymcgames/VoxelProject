using UnityEngine;

public class Vector3IntConvertor
{
    public static long EncodeVector3Int(Vector3Int v)
    {
        return ((long)v.x << 32) | ((long)v.y << 16) | (long)v.z;
    }

    public static Vector3Int DecodeVector3Int(long encoded)
    {
        int x = (int)(encoded >> 32);
        int y = (int)((encoded >> 16) & 0xFFFF);
        int z = (int)(encoded & 0xFFFF);
        return new Vector3Int(x, y, z);
    }
}