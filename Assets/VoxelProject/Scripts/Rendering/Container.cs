using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Container : MonoBehaviour
{
    public Vector3 containerPosition;

    public NoiseBuffer data;
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void Initialize(Material mat, Vector3 position)
    {
        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;
    }

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh()
    {
        meshData.ClearData();
        GenerateMesh();
        UploadMesh();
    }

    public void GenerateMesh()
    {
        Vector3Int voxelBlockPosition;
        Voxel block;

        int counter = 0;
        Vector3Int[] faceVertices = new Vector3Int[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        foreach (VoxelCell nextVoxel in WorldManager.Instance.sourceData)
        {
            voxelBlockPosition = new Vector3Int(nextVoxel.x, nextVoxel.y, nextVoxel.z);
            if (nextVoxel.x < 0 || nextVoxel.y < 0 || nextVoxel.z < 0)
            {
                continue;
            }

            // Skip this voxel if it's below the visibility threshold
            if (int.Parse(nextVoxel.color) <= WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold)
                continue;

            // original
            /*            block = this[voxelBlockPosition];
                        //Only check on solid blocks
                        if (!block.isSolid)
                        {
                            Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + nextVoxel.x + "," + nextVoxel.z + "," + nextVoxel.z + "]");
                            continue;
                        }*/

            float grayScaleValue = float.Parse(nextVoxel.color)/255f;
            voxelColor = new VoxelColor(grayScaleValue,grayScaleValue,grayScaleValue);
            voxelColorAlpha = voxelColor.color;
            voxelColorAlpha.a = 1;
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);


            if (nextVoxel.isSegmentVoxel)
            {             
                CreateClickableVoxel(new Vector3Int(nextVoxel.x,nextVoxel.y,nextVoxel.z));
            }

            //Iterate over each face direction
            for (int i = 0; i < 6; i++)
            {
                //Check if there's a solid block against this face
                //if (checkVoxelIsSolid(voxelBlockPosition + voxelFaceChecks[i]))
                // if (checkVoxelIsSolid(voxelBlockPosition))                
                //     continue;
                //if ( float.Parse(nextVoxel.color) < 18)
                //    continue;

                //Draw this face

                //Collect the appropriate vertices from the default vertices and add the block position
                for (int j = 0; j < 4; j++)
                {
                    faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + voxelBlockPosition;
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

    void CreateClickableVoxel(Vector3Int clickableVoxelPosition)
    {
        // Create a new GameObject for the clickable voxel
        GameObject voxel = new GameObject("Voxel");
        voxel.transform.position = clickableVoxelPosition;

        // Add a BoxCollider to the voxel
        BoxCollider boxCollider = voxel.AddComponent<BoxCollider>();
        boxCollider.size = WorldManager.Instance.voxelMeshConfigurationSettings.standardVoxelSize;

        VoxelClickHandler clickHandler = voxel.AddComponent<VoxelClickHandler>();
        clickHandler.sceneToLoad = "Zooming";

        // Set the voxel GameObject as a child of the original mesh GameObject for organization
        voxel.transform.parent = this.transform;
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
    public bool checkVoxelIsSolid(Vector3 point)
    {
        if (point.y + 2 < 0 || (point.x > WorldManager.WorldSettings.maxWidthX + 2) || (point.z > WorldManager.WorldSettings.maxDepthZ + 2))
            return true;
        else
            // We can make use of an array of Vector locations, used as an index - need to update the NoiseBuffer class.
            return true; // Temp!!! this[point].isSolid;
    }

    // We can make use of an array of Vector locations, used as an index - need to update the NoiseBuffer class.
    //public Voxel this[Vector3 index]
    public Voxel this[int index]
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
    #endregion
}

