using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Container : MonoBehaviour
{
    Dictionary<Vector3Int, Voxel> visibleVoxels = null;

    private Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward, Vector3Int.back,
        Vector3Int.left, Vector3Int.right,
        Vector3Int.up, Vector3Int.down
    };

    public Camera mainCamera;

    public Vector3 containerPosition;

    public NoiseBuffer data;
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private Coroutine distanceCheckCoroutine;

    // Update loop interval to check the camera distance
    private float checkInterval = 0.1f;
    float distanceToCamera = 0f;
    float maxDistance = 200f;

    Vector3 chunkDimensions = new Vector3(WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize);

    int meshCounter = 0;
    int voxelsSelected = 0;
    VoxelColor voxelColor;
    Color voxelColorAlpha;
    Vector2 voxelSmoothness;
    Vector3Int[] faceVertices = new Vector3Int[4];
    Vector2[] faceUVs = new Vector2[4];
    FrustumCulling frustrumCullingCalculator;

    float nearClippingDistance = WorldManager.Instance.worldSettings.nearClippingDistance;

    public void Initialize(Material mat, Vector3 position)
    {
        mainCamera = Camera.main;
        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;

        // Pre-calculate the Frustrum planes so that it isn't calculated each loop!
        frustrumCullingCalculator = new FrustumCulling(mainCamera, 8);
    }

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh()
    {
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

    public void GenerateMesh()
    {
        voxelsSelected = 0;
        meshCounter = 0;
        faceVertices = new Vector3Int[4];
        faceUVs = new Vector2[4];

        Dictionary<Vector3Int, Voxel> visibleVoxels = GetVisibleVoxels(mainCamera);

        foreach (var nextVoxel in visibleVoxels)
        {
            AddVoxelIntoRenderMesh(nextVoxel.Key, nextVoxel.Value);
        }
        WorldManager.Instance.voxelsSelected = voxelsSelected;
    }

    private int AddVoxelIntoRenderMesh(Vector3Int voxelPosition, Voxel voxel)
    {
        if (voxelPosition.x < 0 || voxelPosition.y < 0 || voxelPosition.z < 0)
        {
            return voxelsSelected;
        }

        if (checkVoxelIsSolid(voxelPosition, ref visibleVoxels) == false)
        {
            return voxelsSelected;
        }

        float grayScaleValue = float.Parse(voxel.colorR.ToString()) / 255f;
        voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
        voxelColorAlpha.a = 1;
        voxelColorAlpha = voxelColor.color;
        voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

        if (voxel.isSegmentVoxel)
        {
            CreateClickableVoxel(voxelPosition);
        }

        //Iterate over each face direction
        for (int i = 0; i < 6; i++)
        {
            //Collect the appropriate vertices from the default vertices and add the block Position
            for (int j = 0; j < 4; j++)
            {
                faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + voxelPosition;
                faceUVs[j] = voxelUVs[j];
            }

            for (int j = 0; j < 6; j++)
            {
                meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                meshData.UVs.Add(faceUVs[voxelTris[i, j]]);
                meshData.colors.Add(voxelColorAlpha);
                meshData.UVs2.Add(voxelSmoothness);

                meshData.triangles.Add(meshCounter++);
            }
        }
        return voxelsSelected++;
    }

    float AdjustGrayscale(float originalGrayscale, float distance)
    {
        // Clamp the distance to the range [0, maxDistance]
        distance = Mathf.Clamp(distance, 0, maxDistance);

        // Normalize the distance to the range [0, 1]
        float normalizedDistance = distance / maxDistance;

        // Target grayscale value at minimum distance (e.g., closer to white)
        float targetGrayscaleValue = 254f;

        // Blend the original grayscale value with the target grayscale value based on the normalized distance
        float adjustedGrayscale = Mathf.Lerp(originalGrayscale, targetGrayscaleValue, 1 - normalizedDistance);

        return adjustedGrayscale;
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

    public void UploadMesh()
    {
        meshData.UploadMesh();

        if (meshRenderer == null)
        {
            ConfigureComponents();
        }

        meshFilter.mesh = meshData.mesh;
    }

    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private Dictionary<Vector3Int, Voxel> GetVisibleVoxels(Camera camera)
    {
        // This will hold the voxels that are ultimately doing to be visible.
        visibleVoxels = new Dictionary<Vector3Int, Voxel>(WorldManager.Instance.voxelDictionary.Count, new FastVector3IntComparer());

        foreach (var chunk in WorldManager.Instance.voxelChunks)
        {
            if (frustrumCullingCalculator.IsChunkInView(ref chunk.Value.bounds)) //camera, chunkPosition, chunkDimensions))
            {
                // Only check individual voxels within the chunk if the chunk is visible
                TraverseVisibleChunk(ref camera, chunk.Key, ref visibleVoxels);
            }
        }

        // Traverse(camera, visibleVoxels);

        frustrumCullingCalculator.DropFrustrumPlanes();        

        return visibleVoxels;
    }

    private void TraverseVisibleChunk(ref Camera camera, Vector3Int chunkPosition, ref Dictionary<Vector3Int, Voxel> visibleVoxels)
    {       
        Chunk chunkValue = null;
        if (WorldManager.Instance.voxelChunks.TryGetValue(chunkPosition, out chunkValue) == false)
            return;

        foreach(var nextVoxelInChunk in chunkValue.voxels)
        {
            if (frustrumCullingCalculator.IsVoxelInView(camera, nextVoxelInChunk.Key, nearClippingDistance))
            {
                visibleVoxels[nextVoxelInChunk.Key] = nextVoxelInChunk.Value;
            }
        }
    }

    private void Traverse(Camera camera, Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        foreach (var nextVoxel in WorldManager.Instance.voxelDictionary)
        {
            if (frustrumCullingCalculator.IsVoxelInView(camera, nextVoxel.Key, nearClippingDistance))
            {
                visibleVoxels[nextVoxel.Key] = nextVoxel.Value;
            }
        }
    }

    private bool checkVoxelIsSolid(Vector3Int voxelPosition, ref Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        foreach (var dir in directions)
        {
            if (!visibleVoxels.ContainsKey(voxelPosition + dir))
            {
                // If any neighbour is missing, the voxel is visible
                return true;
            }
        }
        return false;
    }

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
}

