using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

// The most important class in the project as it is effectively the voxel engine
public class Container : MonoBehaviour
{
    //Dictionaries to hold the voxel data
    Dictionary<Vector3Int, Voxel> visibleVoxels = null; //Voxels currently visible to the camera
    Dictionary<Vector3Int, Voxel> worldVoxels = null; //All voxels in the world
    Dictionary<Vector3Int, Chunk> worldChunks = null; //All chunks in the world

    // Directions used for checking neighboring voxels (forward, back, left, right, up, down)
    private static readonly Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward, Vector3Int.back,
        Vector3Int.left, Vector3Int.right,
        Vector3Int.up, Vector3Int.down
    };

    public Camera mainCamera; //Reference to the main camera

    public Vector3 containerPosition; //Position of the container

    public NoiseBuffer data; //Reference to the noise buffer data
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer; // Reference to the mesh renderer
    private MeshFilter meshFilter; // Reference to the mesh filter

    private int visibilityThreshold = 0; //Threshold for voxel visibility slider
    private bool sparseVoxels = false; //Toggle value for sparse voxels (where the voxels are scattered like the spine)

    private int chunksOnDisplay = 0; //Initialise how many chunks are currently visible

    int meshCounter = 0; //Counter for the mesh vertices
    int voxelsSelected = 0;
    private Color voxelColorAlpha;
    private Vector2 voxelSmoothness;
    private Vector3Int[] faceVertices = new Vector3Int[4];
    private Vector2[] faceUVs = new Vector2[4];
    private FrustumCulling frustumCullingCalculator; //Frustrum culling calculator

    private bool grayScaleMode = true;

    private float nearClippingDistance = WorldManager.Instance.worldSettings.nearClippingDistance;

    public void Initialize(Material mat, Vector3 position)
    {
        worldVoxels = WorldManager.Instance.voxelDictionary;
        worldChunks = WorldManager.Instance.voxelChunks;

        // Aim the camera at the most central position in all the chunks.
        mainCamera = Camera.main;
        Camera.main.transform.position = WorldManager.Instance.worldSettings.intialCameraPosition;
        Camera.main.transform.LookAt(WorldManager.Instance.worldSettings.intialCameraPosition);

        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;

        // Pre-calculate the Frustrum planes so that it isn't calculated each loop!
        frustumCullingCalculator = new FrustumCulling(mainCamera, 8);

        // Create a color vector that represents a metallic gleam
        voxelSmoothness = new Vector2(0.0f, 0.0f);
    }

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh()
    {
        visibilityThreshold = WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold;
        grayScaleMode = WorldManager.Instance.worldSettings.grayScaleMode;
        sparseVoxels = WorldManager.Instance.worldSettings.sparseVoxels;
        SCManager.Instance.reRenderingMesh = true;
        meshData.ClearData();
        GenerateMesh();
        UploadMesh();
        SCManager.Instance.reRenderingMesh = false;     
    }

    public void ReRenderMesh()
    {
        RenderMesh();
        Debug.Log("voxel generation done!");
    }

    //Generates mesh data based on currently visible voxels

    public void GenerateMesh()
    {        
        voxelsSelected = 0; //Reset the number of voxels selected
        meshCounter = 0; //Reset the mesh counter
        faceVertices = new Vector3Int[4];
        faceUVs = new Vector2[4];

        Dictionary<Vector3Int, Voxel> visibleVoxels = GetVisibleVoxels(mainCamera); //Retrieve the visible voxels based on the camera position

        // Pre-allocate capacity to avoid resizing lists
        meshData.vertices.Capacity = visibleVoxels.Count * 24; // 6 faces * 4 vertices per face
        meshData.triangles.Capacity = visibleVoxels.Count * 36; // 6 faces * 6 indices per face
        meshData.UVs.Capacity = visibleVoxels.Count * 24;
        meshData.UVs2.Capacity = visibleVoxels.Count * 24;
        meshData.colors.Capacity = visibleVoxels.Count * 24;

        //Iterate over each visible voxel and add it to the render mesh

        foreach (var nextVoxel in visibleVoxels.Values.ToList<Voxel>())
        {
            AddVoxelIntoRenderMesh(nextVoxel.position, nextVoxel);
        }
        WorldManager.Instance.voxelsSelected = voxelsSelected;
    }

    private void AddVoxelIntoRenderMesh(Vector3Int voxelPosition, Voxel voxel)
    {
        if (voxelPosition.x < 0 || voxelPosition.y < 0 || voxelPosition.z < 0)
        {
            return;
        }

        // Skip voxels that are below the visibility threshold
        if (voxel.colorGrayScale <= visibilityThreshold)
        {
            return;
        }

        if (checkVoxelIsSolid(voxel, visibleVoxels) == false) //Check if the voxel is solid (i.e., has at least one neighbor)
        {
            return;
        }
        // Determine the voxel color based on the grayscale value
        if (grayScaleMode)
        {
            voxelColorAlpha = voxel.colorInGrayScale();
        }
        else
        {
            voxelColorAlpha = voxel.color();
        }
        voxelColorAlpha.a = 1; //Set the alpha value to 1 (fully opaque)

        if (voxel.isSegmentVoxel)
        {
            CreateClickableVoxel(voxelPosition);
        }

        // Iterate over each face direction to construct the voxel mesh
        for (int i = 0; i < 6; i++)
        {
            // Collect the vertices and UVs for the face
            for (int j = 0; j < 4; j++)
            {
                faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + voxelPosition;
                faceUVs[j] = voxelUVs[j];
            }

            // Add the vertices, UVs, and colors to the mesh data
            for (int j = 0; j < 6; j++)
            {
                int vertexIndex = voxelTris[i, j];
                meshData.vertices.Add(faceVertices[vertexIndex]);
                meshData.UVs.Add(faceUVs[vertexIndex]);
                meshData.colors.Add(voxelColorAlpha);
                meshData.UVs2.Add(voxelSmoothness);
                meshData.triangles.Add(meshCounter++);
            }
        }
        voxelsSelected++;
    }


    void CreateClickableVoxel(Vector3Int clickableVoxelPosition)
    {
        string voxelTag = "SegmentOne";

        // Create a new GameObject for the clickable voxel
        GameObject voxel = new GameObject("Voxel");
        voxel.transform.position = clickableVoxelPosition;
        voxel.tag = voxelTag;

        // Add a BoxCollider to the voxel
        BoxCollider boxCollider = voxel.AddComponent<BoxCollider>();
        boxCollider.size = WorldManager.Instance.voxelMeshConfigurationSettings.standardVoxelSize;

        VoxelClickHandler clickHandler = voxel.AddComponent<VoxelClickHandler>();
        clickHandler.sceneToLoad = "Zooming";

        // Set the cursor handler so that we get a nice cursor when mouseover/out.
        CustomCursorHandler cch = voxel.AddComponent<CustomCursorHandler>();
        cch.targetTag = voxelTag;

        // Set the voxel GameObject as a child of the original mesh GameObject for organization
        voxel.transform.parent = this.transform;
    }

    public void UploadMesh() //Sends the mesh data(verts, tris, UVs, colors) to the GPU
    {
        meshData.UploadMesh();

        //Prevent null reference exceptions
        if (meshRenderer == null)
        {
            ConfigureComponents();
        }

        meshFilter.mesh = meshData.mesh; //assign the newly created mesh to the mesh filter
    }

    private void ConfigureComponents()
    {
        //Needs the components to be assigned on the game object
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private Dictionary<Vector3Int, Voxel> GetVisibleVoxels(Camera camera)
    {
        chunksOnDisplay = 0;

        // This will hold the voxels that are ultimately doing to be visible.
        visibleVoxels = new Dictionary<Vector3Int, Voxel>(worldVoxels.Count, new FastVector3IntComparer());

        foreach (var chunk in worldChunks.Values.ToList<Chunk>())
        {
            if (frustumCullingCalculator.IsChunkInView(ref chunk.bounds)) //Check if the chunk is visible
            {
                // Only check individual voxels within the chunk if the chunk is visible
                TraverseVisibleChunk(ref camera, chunk.chunkPosition, ref visibleVoxels);
                chunksOnDisplay++; //Increments the number of chunks on display
            }
        }

        frustumCullingCalculator.DropFrustrumPlanes(); //Clear the frustum planes when not in use
        WorldManager.Instance.chunksOnDisplay = chunksOnDisplay; //Stats for panel
        
        return visibleVoxels;
    }

    private void TraverseVisibleChunk(ref Camera camera, Vector3Int chunkPosition, ref Dictionary<Vector3Int, Voxel> visibleVoxels)
    {       
        Chunk chunkValue = null;
        //If the chunk is not found in the dictionary, exit the method early
        if (worldChunks.TryGetValue(chunkPosition, out chunkValue) == false)
            return;
        //Iterate over each voxel in the chunk (converted to a list from the voxel dictionary)
        foreach (var nextVoxelInChunk in chunkValue.voxels.Values.ToList<Voxel>())
        {
            //Check if the voxel is within the camera's frustum (visible area) using the frustrum culling calculator
            if (frustumCullingCalculator.IsVoxelInView(camera, nextVoxelInChunk.position, nearClippingDistance))
            {
                //If the voxel is within the frustum, add it to the visible voxels dictionary
                visibleVoxels[nextVoxelInChunk.position] = nextVoxelInChunk;
            }
        }

    }
    //Check if the voxel is solid (i.e., has at least one neighbor) as not worth rendering if its hidden
    private bool checkVoxelIsSolid(Voxel voxelPosition, Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        if (sparseVoxels == true)
        {
            return true;
        }

        for (int i = 0; i < directions.Length; i++)
        {
            // Check if the neighboring voxel is missing (or not visible)
            if (!visibleVoxels.TryGetValue(voxelPosition.position + directions[i], out var outVoxel))
            {
                // If any neighbor is missing, the voxel is visible
                return true;
            }
        }
        return false;
    }
    //Mesh data structure to hold the mesh data
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
                vertices = new List<Vector3>(WorldManager.Instance.voxelDictionary.Count);
                triangles = new List<int>(WorldManager.Instance.voxelDictionary.Count);
                UVs = new List<Vector2>(WorldManager.Instance.voxelDictionary.Count);
                UVs2 = new List<Vector2>(WorldManager.Instance.voxelDictionary.Count);
                colors = new List<Color>(WorldManager.Instance.voxelDictionary.Count);

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
        //Uploads the mesh data to the GPU
        //Based off voxel tutorials
        public void UploadMesh(bool sharedVertices = false)
        {
            try
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
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.Log("Error: colors="+colors.Count);
                Debug.Log("Error: vertices=" + vertices.Count);
            }
        }
    }
    //Based off voxel tutorials, theese are tables that define the triangles and vertices of the voxel faces and are used to contruct the elements of the voxel mesh,
    //that is rendered to the screen
    static readonly Vector3Int[] voxelVertices = new Vector3Int[8]
    {
            new Vector3Int(0,0,0),//0
            new Vector3Int(1,0,0),//1
            new Vector3Int(0,1,0),//2
            new Vector3Int(1,1,0),//3

            new Vector3Int(0,0,1),//4
            new Vector3Int(1,0,1),//5
            new Vector3Int(0,1,1),//6
            new Vector3Int(1,1,1),//7
    };

    static readonly Vector3Int[] voxelFaceChecks = new Vector3Int[6]
    {
            new Vector3Int(0,0,-1),//back
            new Vector3Int(0,0,1),//front
            new Vector3Int(-1,0,0),//left
            new Vector3Int(1,0,0),//right
            new Vector3Int(0,-1,0),//bottom
            new Vector3Int(0,1,0)//top
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

    //UVs for the voxel faces
    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
    };

    //Triangle indices for the voxel faces
    static readonly int[,] voxelTris = new int[6, 6]
    {
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
    };
}

