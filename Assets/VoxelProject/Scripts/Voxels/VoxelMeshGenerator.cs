using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public static class VoxelMeshGenerator
{
    private static Vector3Int voxelSize = new Vector3Int(1, 1, 1);

    private static List<Vector3> vertices = new List<Vector3>();
    private static List<int> triangles = new List<int>();
    private static List<Vector2> uvs = new List<Vector2>();
    private static List<Vector2> uv2s = new List<Vector2>();
    private static List<Color> colors = new List<Color>();

    private static Transform internalTransform = null;
    private static int layer = 0;

    public static Container.MeshData GenerateMesh(VoxelChunk chunk, ref Container.MeshData meshData, Transform transform)
    {
        // If this chunk is totally empty, just drop out right away as we're wasting time!
        if (chunk.hasAtLeastOneActiveVoxel == false)
        {
            Debug.Log("CHUNK ["+chunk.chunkCoordinates+"] empty, no need to render it!");
            return meshData;
        }

        internalTransform = transform;
/*        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();*/
        
        for (int y = 0; y < WorldManager.Instance.voxelGrid.chunkSize; y++)
        {
            for (int x = 0; x < WorldManager.Instance.voxelGrid.chunkSize; x++)
            {
                for (int z = 0; z < WorldManager.Instance.voxelGrid.chunkSize; z++)
                {
                    Vector3Int localPosition = new Vector3Int(x, y, z);
                    Voxel voxel = chunk.GetVoxel(localPosition);
                    if (voxel != null && voxel.isActive)
                    {
                        Vector3Int position = new Vector3Int(chunk.chunkCoordinates.x * WorldManager.Instance.voxelGrid.chunkSize + x, chunk.chunkCoordinates.y * WorldManager.Instance.voxelGrid.chunkSize + y, chunk.chunkCoordinates.z * WorldManager.Instance.voxelGrid.chunkSize + z);
                        //AddVoxel(vertices, triangles, uv, position);
                        //ProcessVoxel(position.x, position.y, position.z);
                        Debug.Log("Rendered voxel: " + voxel.position);
                    }
                    else if (voxel != null)
                    {
                        Debug.Log("Skipped voxel: " + voxel.position);
                    }
                }
            }
        }

        meshData.vertices = vertices;//.Add(faceVertices[voxelTris[i, j]]);
        //Debug.Log("vertices size:" + vertices.Count);
        meshData.UVs = uvs;//.Add(faceUVs[voxelTris[i, j]]);
        meshData.triangles = triangles;//.Add(counter++);
        meshData.colors = colors;
        //Debug.Log("colors size:" + colors.Count);
        meshData.UVs2 = uv2s;
        /*        Mesh mesh = new Mesh();
                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();
                mesh.uv = uv.ToArray();
                mesh.RecalculateNormals();

                meshData.mesh = mesh;
        */
        return meshData;
    }

    private static void ProcessVoxel(int x, int y, int z)
    {
        // KJP TODO
        // Check if the voxels array is initialized and the indices are within bounds
/*        if (WorldManager.Instance.sourceData == null || x < 0 || x >= WorldManager.Instance.sourceData.GetLength(0) ||
            y < 0 || y >= WorldManager.Instance.sourceData.GetLength(1) || z < 0 || z >= WorldManager.Instance.sourceData.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }
*/
        Voxel voxel = WorldManager.Instance.voxelGrid.GetVoxel(new Vector3Int(x, y, z));
        if (voxel.isSolid && voxel.getColourRGBLayer(layer) > (int)WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i); // Method to add mesh data for the visible face
            }
        }
    }

    private static bool IsFaceVisible(int x, int y, int z)
    {
        // Check if the neighboring voxel is inactive or out of bounds in the current chunk
        // and also if it's inactive or out of bounds in the world (neighboring chunks)
        return IsVoxelHiddenInChunk(x, y, z);// && IsVoxelHiddenInWorld(globalPos);

    }

    private static bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        try
        {
            if (WorldManager.Instance.voxelGrid.GetVoxel(new Vector3Int(x, y, z)) == null)
                return true;
            if (x < 0 || x >= WorldManager.Instance.worldSettings.chunkSize || y < 0 || y >= WorldManager.Instance.worldSettings.chunkSize || z < 0 || z >= WorldManager.Instance.worldSettings.chunkSize)
                return true; // Face is at the boundary of the chunk
            return !WorldManager.Instance.voxelGrid.GetVoxel(new Vector3Int(x, y, z)).isSolid;
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.Log("x: " + x + ", y: " + y + ", z: " + z + " is out of range");
            Debug.Log("Exception: " + e.Message);
            return true;
        }
    }

    private static void AddFaceData(int x, int y, int z, int faceIndex)
    {
        Voxel voxel = WorldManager.Instance.voxelGrid.GetVoxel(new Vector3Int(x, y, z));

        VoxelColor voxelColor = null;

/*        if (voxel.isHotVoxel())
        {
            //Debug.Log("Hot one found!");
            //colourValue = ;
            voxelColor = BuildColour(voxel.getHotVoxelColourRGB());

            CreateClickableVoxel(x, y, z);
        }
        else */
//        if (voxel.isGreyScale())
//        {
            // It's a greyscale colour!
            //            colourValue = ;
            //Debug.Log("Greyscale found ("+layer+")! [" + voxel.getColourRGBLayer(layer) / 255f + "]");
            voxelColor = new VoxelColor(voxel.getColourRGBLayer(layer) / 255f, voxel.getColourRGBLayer(layer) / 255f, voxel.getColourRGBLayer(layer) / 255f);
            //voxelColor = new VoxelColor(voxel.getColourRGBLayer(layer), voxel.getColourRGBLayer(layer), voxel.getColourRGBLayer(layer));
        /*        }
                else
                {
                    Debug.Log("Colourful one found!");
                    // Handle the regular colour
                    //colourValue = ;
                    voxelColor = BuildColour(WorldManager.Instance.voxelGrid.GetVoxel(new Vector3Int(x, y, z)).getColourRGBLayer(layer));
                }
        */
        Color voxelColorAlpha = voxelColor.color;
        voxelColorAlpha.a = 1;
        Vector2 voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
        colors.Add(voxelColorAlpha);
        colors.Add(voxelColorAlpha);
        colors.Add(voxelColorAlpha);
        colors.Add(voxelColorAlpha);
        uv2s.Add(voxelSmoothness);
        uv2s.Add(voxelSmoothness);
        uv2s.Add(voxelSmoothness);
        uv2s.Add(voxelSmoothness);

        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));
        }
        AddTriangleIndices();
    }

    private static void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }

    private static VoxelColor BuildColour(float colourValue)
    {
        VoxelColor voxelColor;
        Color color;
        if (ColorUtility.TryParseHtmlString(convertIntToHTMLColour(colourValue), out color))
        {
            float red = color.r;
            float green = color.g;
            float blue = color.b;
            //Debug.Log($"Red: {red}, Green: {green}, Blue: {blue}");
            voxelColor = new VoxelColor(red, green, blue);
        }
        else
        {
            voxelColor = new VoxelColor(25, 25, 25);
        }

        return voxelColor;
    }

    private static string convertIntToHTMLColour(float colour)
    {
        int red = ((int)colour >> 16) & 0xFF;
        int green = ((int)colour >> 8) & 0xFF;
        int blue = (int)colour & 0xFF;

        // Format as a hex string
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    static void CreateClickableVoxel(int x, int y, int z)
    {
        // Create a new GameObject for the voxel
        GameObject voxel = new GameObject("Voxel");
        voxel.transform.position = new Vector3Int(x, y, z);

        // Add a BoxCollider to the voxel
        BoxCollider boxCollider = voxel.AddComponent<BoxCollider>();
        boxCollider.size = voxelSize;

        VoxelClickHandler clickHandler = voxel.AddComponent<VoxelClickHandler>();
        clickHandler.sceneToLoad = "SwitchSceneOne";

        /*        // Optionally, you can add a MeshRenderer and MeshFilter to visualize the voxel
                MeshRenderer meshRenderer = voxel.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = voxel.AddComponent<MeshFilter>();
                meshFilter.mesh = CreateVoxelMesh();*/

        // Set the voxel GameObject as a child of the original mesh GameObject for organization
        voxel.transform.parent = internalTransform;
    }

    private static void AddVoxel(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, Vector3 position)
    {
        int vertexIndex = vertices.Count;

        // Add the 8 vertices of the cube
        vertices.Add(position + new Vector3(0, 0, 0));
        vertices.Add(position + new Vector3(1, 0, 0));
        vertices.Add(position + new Vector3(1, 1, 0));
        vertices.Add(position + new Vector3(0, 1, 0));
        vertices.Add(position + new Vector3(0, 0, 1));
        vertices.Add(position + new Vector3(1, 0, 1));
        vertices.Add(position + new Vector3(1, 1, 1));
        vertices.Add(position + new Vector3(0, 1, 1));

        // Create the 6 faces of the cube (2 triangles per face)
        // Front face
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 2); triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 3); triangles.Add(vertexIndex + 2);

        // Back face
        triangles.Add(vertexIndex + 4); triangles.Add(vertexIndex + 5); triangles.Add(vertexIndex + 6);
        triangles.Add(vertexIndex + 4); triangles.Add(vertexIndex + 6); triangles.Add(vertexIndex + 7);

        // Top face
        triangles.Add(vertexIndex + 3); triangles.Add(vertexIndex + 7); triangles.Add(vertexIndex + 6);
        triangles.Add(vertexIndex + 3); triangles.Add(vertexIndex + 6); triangles.Add(vertexIndex + 2);

        // Bottom face
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 1); triangles.Add(vertexIndex + 5);
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 5); triangles.Add(vertexIndex + 4);

        // Left face
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 4); triangles.Add(vertexIndex + 7);
        triangles.Add(vertexIndex + 0); triangles.Add(vertexIndex + 7); triangles.Add(vertexIndex + 3);

        // Right face
        triangles.Add(vertexIndex + 1); triangles.Add(vertexIndex + 2); triangles.Add(vertexIndex + 6);
        triangles.Add(vertexIndex + 1); triangles.Add(vertexIndex + 6); triangles.Add(vertexIndex + 5);

        // UV mapping (assuming each face of the voxel uses the same texture)
        uv.AddRange(new Vector2[] {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        });
    }
}