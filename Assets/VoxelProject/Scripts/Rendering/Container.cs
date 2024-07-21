using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Container : MonoBehaviour
{
    public Vector3 containerPosition;

    private int layer;

    public NoiseBuffer data;
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public VoxelGrid voxelGrid;
    public GameObject voxelPrefab;
    public Camera mainCamera;

    public void Initialize(Material mat, Vector3 position)
    {
        Debug.Log("Initialising Container!");
        mainCamera = Camera.main;
        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;
    }

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderVisibleChunks(int layer)
    {        
        this.layer = layer;
        meshData.ClearData();

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (var chunk in WorldManager.Instance.voxelGrid.Chunks.Values)
        {
            //if (GeometryUtility.TestPlanesAABB(frustumPlanes, chunk.chunkBounds))
            //{
                // Chunk is within the camera's frustum, render it
                RenderChunk(chunk);
            //}
        }

        //GenerateMesh();
        //UploadMesh();
    }

    void RenderChunk(VoxelChunk chunk)
    {
        Transform innerTransform = this.transform;
        meshData = VoxelMeshGenerator.GenerateMesh(chunk, ref meshData, innerTransform);
        UploadMesh();
        //GameObject chunkObject = new GameObject("Chunk " + chunk.chunkCoordinates);
        //chunkObject.transform.SetParent(transform);
        //MeshFilter mf = chunkObject.AddComponent<MeshFilter>();
        //MeshRenderer mr = chunkObject.AddComponent<MeshRenderer>();
        //mf.mesh = meshData.mesh;
        //mr.material = new Material(Shader.Find("Standard"));
    }

    public void GenerateMesh(VoxelChunk chunk)
    {
        Debug.Log("Generating mesh for layer: " + layer);

        //int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        int breaker = 0;

        for (int x=0; x<WorldManager.Instance.worldSettings.maxWidthX-1; x++) 
        {
            for (int y=0; y<WorldManager.Instance.worldSettings.maxHeightY-1; y++)
            {
                for (int z=0; z<WorldManager.Instance.worldSettings.maxDepthZ-1; z++)
                {
                    // if (breaker >= 10000)
                    //     break;
                    breaker++;

                    // KJPProcessVoxel(x, y, z);

                    /*                    block = WorldManager.Instance.sourceData[x, y, z];

                                        //Only check on solid blocks
                                        if (!block.isSolid)
                                        {
                                            Debug.Log("Non solid block encountered (Loop-" + breaker + ")! [" + x + "," + y + "," + z + "]");
                                            continue;
                                        }

                                        float colourValue = block.colourRGBValue / 255f;
                                        voxelColor = new VoxelColor(colourValue, colourValue, colourValue);

                                        voxelColorAlpha = voxelColor.color;
                                        voxelColorAlpha.a = 1;
                                        voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                                        //Iterate over each face direction
                                        for (int i = 0; i < 6; i++)
                                        {
                                            //Check if there's a solid block against this face
                                            //if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                                            // if (checkVoxelIsSolid(blockPos))                
                                            //     continue;

                                            //Draw this face

                                            //Collect the appropriate vertices from the default vertices and add the block position
                                            for (int j = 0; j < 4; j++)
                                            {
                                                faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + new Vector3(x,y,z);
                                                faceUVs[j] = voxelUVs[j];
                                            }

                                            for (int j = 0; j < 6; j++)
                                            {
                                                meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                                                meshData.UVs.Add(faceUVs[voxelTris[i, j]]);
                                                meshData.colors.Add(voxelColorAlpha);
                                                meshData.UVs2.Add(voxelSmoothness);

                                                meshData.triangles.Add(counter++);

                                            }
                                        }*/
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

        /* oldway       foreach (VoxelCell vc in WorldManager.Instance.sourceData)
                {
                    // if (breaker >= 10000)
                    //     break;
                    breaker++;

                    blockPos = new Vector3(vc.widthX, vc.heightY, vc.depthZ);
                    if (vc.widthX < 0 || vc.heightY < 0 || vc.depthZ < 0)
                    {
                        Debug.Log("Weird voxel encountered (Loop-"+breaker+")! [" + vc.widthX + "," + vc.depthZ + "," + vc.depthZ + "]");
                        continue;
                    }
                    block = this[blockPos];
                    //Only check on solid blocks
                    if (!block.isSolid)
                    {
                        Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + vc.widthX + "," + vc.depthZ + "," + vc.depthZ + "]");
                        continue;
                    }

                    float colourValue = float.Parse(vc.value)/255f;
                    voxelColor = new VoxelColor(colourValue,colourValue,colourValue);

                    voxelColorAlpha = voxelColor.color;
                    voxelColorAlpha.a = 1;
                    voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                    //Iterate over each face direction
                    for (int i = 0; i < 6; i++)
                    {
                        //Check if there's a solid block against this face
                        //if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                        // if (checkVoxelIsSolid(blockPos))                
                        //     continue;
                        if ( float.Parse(vc.value) < 18)
                            continue;

                        //Draw this face

                        //Collect the appropriate vertices from the default vertices and add the block position
                        for (int j = 0; j < 4; j++)
                        {
                            faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + blockPos;
                            faceUVs[j] = voxelUVs[j];
                        }

                        for (int j = 0; j < 6; j++)
                        {
                            meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                            meshData.UVs.Add(faceUVs[voxelTris[i, j]]);
                            meshData.colors.Add(voxelColorAlpha);
                            meshData.UVs2.Add(voxelSmoothness);

                            meshData.triangles.Add(counter++);

                        }
                    }
                }*/
    }

    private void ProcessVoxel(int x, int y, int z)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (WorldManager.Instance.sourceData == null || x < 0 || x >= WorldManager.Instance.sourceData.GetLength(0) ||
            y < 0 || y >= WorldManager.Instance.sourceData.GetLength(1) || z < 0 || z >= WorldManager.Instance.sourceData.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }

        VoxelStruct voxel = WorldManager.Instance.sourceData[x, y, z];
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

    private bool IsFaceVisible(int x, int y, int z)
    {
/*        // Check if the neighboring voxel in the given direction is inactive or out of bounds
        if (x < 0 || x >= WorldManager.Instance.worldSettings.chunkSize || y < 0 || y >= WorldManager.Instance.worldSettings.chunkSize || z < 0 || z >= WorldManager.Instance.worldSettings.chunkSize)
            return true; // Face is at the boundary of the chunk
        return !WorldManager.Instance.sourceData[x, y, z].isSolid;*/

        // Convert local chunk coordinates to global coordinates
        Vector3 globalPos = transform.position + new Vector3(x, y, z);

        // Check if the neighboring voxel is inactive or out of bounds in the current chunk
        // and also if it's inactive or out of bounds in the world (neighboring chunks)
        return IsVoxelHiddenInChunk(x, y, z);// && IsVoxelHiddenInWorld(globalPos);

    }

    private bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        try {
            if (x < 0 || x >= WorldManager.Instance.worldSettings.chunkSize || y < 0 || y >= WorldManager.Instance.worldSettings.chunkSize || z < 0 || z >= WorldManager.Instance.worldSettings.chunkSize)
                return true; // Face is at the boundary of the chunk
            return !WorldManager.Instance.sourceData[x, y, z].isSolid;
        } catch (System.IndexOutOfRangeException e) {
            Debug.Log("x: " + x + ", y: " + y + ", z: " + z + " is out of range");
            Debug.Log("Exception: " + e.Message);
            return true;
        }
    }

/*    private bool IsVoxelHiddenInWorld(Vector3 globalPos)
    {
        return false;
        // Check if there is a chunk at the global position
        Chunk neighborChunk = WorldManager.Instance.GetChunkAt(globalPos);
        if (neighborChunk == null)
        {
            // No chunk at this position, so the voxel face should be hidden
            return true;
        }

        // Convert the global position to the local position within the neighboring chunk
        Vector3 localPos = neighborChunk.transform.InverseTransformPoint(globalPos);

        // If the voxel at this local position is inactive, the face should be visible (not hidden)
        return !neighborChunk.IsVoxelActiveAt(localPos);
    }*/

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Vector2> uv2s = new List<Vector2>();
    private List<Color> colors = new List<Color>();

    private string convertIntToHTMLColour(float colour)
    {
        int red = ((int)colour >> 16) & 0xFF;
        int green = ((int)colour >> 8) & 0xFF;
        int blue = (int)colour & 0xFF;

        // Format as a hex string
        return $"#{red:X2}{green:X2}{blue:X2}";
    }


    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        VoxelStruct voxel = WorldManager.Instance.sourceData[x, y, z];

        VoxelColor voxelColor = null;

        if (voxel.isHotVoxel)
        {
            //Debug.Log("Hot one found!");
            //colourValue = ;
            voxelColor = BuildColour(voxel.getHotVoxelColourRGB());

            CreateClickableVoxel(x, y, z);
        }
        else if (voxel.isGreyScale)
        {            
            // It's a greyscale colour!
//            colourValue = ;
            //Debug.Log("Greyscale found ("+layer+")! [" + voxel.getColourRGBLayer(layer) / 255f + "]");
            voxelColor = new VoxelColor(voxel.getColourRGBLayer(layer) / 255f, voxel.getColourRGBLayer(layer) / 255f, voxel.getColourRGBLayer(layer) / 255f);
        }
        else
        {
            //Debug.Log("Colourful one found!");
            // Handle the regular colour
            //colourValue = ;
            voxelColor = BuildColour(WorldManager.Instance.sourceData[x, y, z].getColourRGBLayer(layer));
        }

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

    private VoxelColor BuildColour(float colourValue)
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

    // Voxel size
    public Vector3 voxelSize = new Vector3(1, 1, 1);

/*    void GenerateVoxelColliders()
    {
        // Get the vertices of the mesh
        Vector3[] vertices = meshData.mesh.vertices;

        // For each vertex, create a voxel GameObject
        foreach (Vector3 vertex in vertices)
        {
            CreateClickableVoxel(vertex);
        }
    }*/

    void CreateClickableVoxel(int x, int y, int z)
    {
        // Create a new GameObject for the voxel
        GameObject voxel = new GameObject("Voxel");
        voxel.transform.position = new Vector3Int(x,y,z);

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
        voxel.transform.parent = this.transform;
    }

    private void AddTriangleIndices()
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


    public void UploadMesh()
    {
        meshData.UploadMesh();

        if (meshRenderer == null)
            ConfigureComponents();

        meshFilter.mesh = meshData.mesh;
        if (meshData.vertices.Count > 3)
            meshCollider.sharedMesh = meshData.mesh;
    }

    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = false;
    }
    public bool checkVoxelIsSolid(Vector3Int point)
    {
        if (point.y + 2 < 0 || (point.x > WorldManager.WorldSettings.maxWidthX + 2) || (point.z > WorldManager.WorldSettings.maxDepthZ + 2))
            return true;
        else
            return this[point].isSolid;
    }

    public VoxelStruct this[Vector3Int index]
    {
        get
        {
            return data[index];
        }

        set
        {
            data[index] = value;
        }
    }

    #region Mesh Data

    public struct MeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;
        public List<Vector2> UVs2;
        public List<Color> colors;
        public bool Initialized;

        public void ClearData()
        {
            if (!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();
                UVs2 = new List<Vector2>();
                colors = new List<Color>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                UVs2.Clear();
                colors.Clear();

                mesh.Clear();
            }
        }
        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);

            mesh.SetUVs(0, UVs);
            mesh.SetUVs(2, UVs2);

            mesh.Optimize();

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
    #endregion

    #region Static Variables
    static readonly Vector3[] voxelVertices = new Vector3[8]
    {
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
    };

    static readonly Vector3[] voxelFaceChecks = new Vector3[6]
    {
            new Vector3(0,0,-1),//back
            new Vector3(0,0,1),//front
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
            {0,1,2,3},
            {4,5,6,7},
            {4,0,6,2},
            {5,1,7,3},
            {0,1,4,5},
            {2,3,6,7},
    };

    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
    };
    #endregion
}

