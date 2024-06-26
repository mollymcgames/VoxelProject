using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class OuterContainer : MonoBehaviour
{
    public Vector3 containerPosition;

    public OuterNoiseBuffer data;
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private Camera mainCamera;

    public int chunkInnerSize = OuterWorldManager.Instance.chunkInnerSize;
    public int chunkOuterSize = OuterWorldManager.Instance.chunkOuterSize;

    public int chunkFieldOfViewMultiplierInner = OuterWorldManager.Instance.chunkFieldOfViewMultiplierInner;
    public int chunkFieldOfViewMultiplierOuter = OuterWorldManager.Instance.chunkFieldOfViewMultiplierOuter;


    public void Start(){
        mainCamera = Camera.main;
    }

    public void Initialize(Material mat, Vector3 position)
    {
        ConfigureComponents();
        data = OuterComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;
    }

    public void ClearData()
    {
        OuterComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh(bool renderOuter = true, float transparencyValue = 1f)
    {
        meshData.ClearData();
        GenerateMesh(renderOuter, transparencyValue);
        UploadMesh();
    }

    public void RenderMeshDictionary(bool renderOuter = true, float transparencyValue = 1f)
    {
        meshData.ClearData();
        GenerateMeshDictionary(renderOuter, transparencyValue);
        UploadMesh();
    }

    public void GenerateMesh(bool renderOuter = true, float transparencyValue = 1f)
    {
        Vector3 blockPos;
        Voxel block;

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        VoxelCell[] sourceData = null;
        if (renderOuter)
        {
            sourceData = OuterWorldManager.Instance.sourceDataOuter;
        } else {
            sourceData = OuterWorldManager.Instance.sourceDataInner;
        }
        int breaker = 0;
        //for (VoxelCell vc = 1; x < WorldManager.Instance.widthX + 1; x++)
        foreach (VoxelCell vc in sourceData)
        {
            // if (breaker >= 10000)
            //     break;
            breaker++;

            if (vc == null)
                continue;
            blockPos = new Vector3(vc.widthX, vc.heightY, vc.depthZ);
            // if (vc.widthX < 0 || vc.heightY < 0 || vc.depthZ < 0)
            // {
            //     Debug.Log("Weird voxel encountered (Loop-"+breaker+")! [" + vc.widthX + "," + vc.depthZ + "," + vc.depthZ + "]");
            //     continue;
            // }
            block = this[blockPos];
            //Only check on solid blocks
            if (!block.isSolid)
            {
                Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + vc.widthX + "," + vc.depthZ + "," + vc.depthZ + "]");
                continue;
            }

            // float grayScaleValue = float.Parse(vc.value)/255f;
            // voxelColor = new VoxelColor(grayScaleValue,grayScaleValue,grayScaleValue);
            voxelColor = new VoxelColor();


            Color color;
            if (!ColorUtility.TryParseHtmlString("#" + vc.value, out color))
            {
                Debug.LogError($"Invalid color value in line: {vc.value}");
                continue;
            }            

            voxelColorAlpha = color;
            voxelColorAlpha.a = transparencyValue; //THIS IS WHERE THE ALPHA VALUE IS SET 
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
            //Iterate over each face direction
            for (int i = 0; i < 6; i++)
            {
                //Check if there's a solid block against this face
                //if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                // if (checkVoxelIsSolid(blockPos))                
                //     continue;
                // if ( float.Parse(vc.value) < 18)
                //     continue;

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
        }
    }
    
    private Dictionary<Vector3Int, Chunk> GetSurroundingChunks(Vector3Int position, int chunkDistanceMultiplier, int chunkSize, Dictionary<Vector3Int, Chunk> sourceData)
    {
        Dictionary<Vector3Int, Chunk> renderVectors = new Dictionary<Vector3Int, Chunk>();
        List<Vector3Int> points = new List<Vector3Int>();

        for (int x = -chunkDistanceMultiplier * chunkSize; x <= chunkDistanceMultiplier * chunkSize; x += chunkSize)
        {
            for (int y = -chunkDistanceMultiplier * chunkSize; y <= chunkDistanceMultiplier * chunkSize; y += chunkSize)
            {
                for (int z = -chunkDistanceMultiplier * chunkSize; z <= chunkDistanceMultiplier * chunkSize; z += chunkSize)
                {
                    //if (x == 0 && y == 0 && z == 0) 
                    //    continue; // Skip the current position
                    Vector3Int newPoint = new Vector3Int(position.x + x, position.y + y, position.z + z);
                    sourceData.TryGetValue(newPoint, out Chunk chunk);
                    if ( chunk != null)
                        renderVectors.Add(newPoint, chunk);
                }
            }
        }
        return renderVectors;
    }

    public void GenerateMeshDictionary(bool renderOuter = true, float transparencyValue = 1f)
    {
        Vector3Int blockPos;
        Voxel block;

        int counter = 0;
        Vector3Int[] faceVertices = new Vector3Int[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        Vector3Int chunkCoordinates = Vector3Int.zero;
        Dictionary<Vector3Int, Chunk> renderVectors = null;
        Dictionary<Vector3Int, Chunk> sourceData = null;
        if (renderOuter)
        {
            Debug.Log("[OUTER] We need to render a chunk for this camera position: " + Vector3Int.FloorToInt(mainCamera.transform.position));

            // Using the current camera position, calculate the relevant chunk coordinates.
            // This is going to form the centre point for the selection of chunks we're going to render....
            chunkCoordinates = Chunk.GetChunkCoordinates(Vector3Int.FloorToInt(mainCamera.transform.position), chunkOuterSize);

            sourceData = OuterWorldManager.Instance.sourceDataOuterDictionary;
            // Now using that centre point, get the surrounding chunks.
            // Essentially we're going to end up with effectively a Rubic's cube of chunks with our camera position in the dead centre.
            renderVectors = GetSurroundingChunks(chunkCoordinates, chunkFieldOfViewMultiplierOuter, chunkOuterSize, sourceData);
        }
        else
        {
            Debug.Log("[INNER] We need to render a chunk for this camera position: " + Vector3Int.FloorToInt(mainCamera.transform.position));

            // Using the current camera position, calculate the relevant chunk coordinates.
            // This is going to form the centre point for the selection of chunks we're going to render....
            chunkCoordinates = Chunk.GetChunkCoordinates(Vector3Int.FloorToInt(mainCamera.transform.position), chunkInnerSize);

            sourceData = OuterWorldManager.Instance.sourceDataInnerDictionary;
            // Now using that centre point, get the surrounding chunks.
            // Essentially we're going to end up with effectively a Rubic's cube of chunks with our camera position in the dead centre.
            renderVectors = GetSurroundingChunks(chunkCoordinates, chunkFieldOfViewMultiplierInner, chunkInnerSize, sourceData);

        }
        Debug.Log(renderOuter?"[OUTER]":"[INNER]"+" Number of chunks selected: "+renderVectors.Count );

        int breaker = 0;

        // Now prep those 27 chunks for rendering....
        foreach (KeyValuePair<Vector3Int, Chunk> nextChunk in renderVectors)
        {
            foreach (VoxelElement nextVoxelElement in nextChunk.Value.voxels)
            {
                blockPos = nextVoxelElement.position;
                block = this[blockPos];
                //Only check on solid blocks
                if (!block.isSolid)
                {
                    Debug.Log("Non solid block encountered (Loop-" + breaker + ")! [" + nextVoxelElement.position.x + "," + nextVoxelElement.position.y + "," + nextVoxelElement.position.z + "]");
                    continue;
                }

                // float grayScaleValue = float.Parse(vc.value)/255f;
                // voxelColor = new VoxelColor(grayScaleValue,grayScaleValue,grayScaleValue);
                voxelColor = new VoxelColor();

                Color color;
                if (!ColorUtility.TryParseHtmlString("#" + nextVoxelElement.colorString, out color))
                {
                    Debug.LogError($"Invalid color value in line: {nextVoxelElement.colorString}");
                    continue;
                }

                voxelColorAlpha = color;
                voxelColorAlpha.a = transparencyValue; //THIS IS WHERE THE ALPHA VALUE IS SET 
                voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                //Iterate over each face direction
                for (int i = 0; i < 6; i++)
                {
                    //Check if there's a solid block against this face
                    //if (checkVoxelIsSolid(blockPos + voxelFaceChecks[i]))
                    // if (checkVoxelIsSolid(blockPos))                
                    //     continue;
                    // if ( float.Parse(vc.value) < 18)
                    //     continue;

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
                breaker++;
            }
        }
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
        if (point.y + 2 < 0 || (point.x > OuterWorldManager.WorldSettings.maxWidthX + 2) || (point.z > OuterWorldManager.WorldSettings.maxDepthZ + 2))
            return true;
        else
            return this[point].isSolid;
    }

    public Voxel this[Vector3 index]
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

