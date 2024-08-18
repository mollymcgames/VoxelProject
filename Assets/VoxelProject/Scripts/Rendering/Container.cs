using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]
public class Container : MonoBehaviour
{
    private Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward, Vector3Int.back,
        Vector3Int.left, Vector3Int.right,
        Vector3Int.up, Vector3Int.down
    };

    public Camera mainCamera;

    public Vector3 containerPosition;

    // This adjusts the near clipping distance for our Frustrum.
    public float targetNearClippingDistance = 1.0f;  // Desired clipping distance
    public float adjustmentSpeed = 2.0f;  // Speed of adjustment

    public NoiseBuffer data;
    private MeshData meshData = new MeshData();

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private Coroutine distanceCheckCoroutine;

    // Update loop interval to check the camera distance
    private float checkInterval = 0.1f;
    float distanceToCamera = 0f;

    public void Initialize(Material mat, Vector3 position)
    {
        mainCamera = Camera.main;
        ConfigureComponents();
        data = ComputeManager.Instance.GetNoiseBuffer();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;

        //distanceCheckCoroutine = StartCoroutine(CheckCameraDistanceCoroutine());
    }

/*    IEnumerator CheckCameraDistanceCoroutine()
    {
        while (true)
        {
            distanceToCamera = Vector3.Distance(mainCamera.transform.Position, WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter);
            //AnimateVoxelsBasedOnDistance(distanceToCamera);
            yield return new WaitForSeconds(checkInterval);
        }
    }*/

/*    void AnimateVoxelsBasedOnDistance(float distanceToCamera)
    {
        foreach (VoxelCell voxel in WorldManager.Instance.voxelDictionary)
        {
            if (voxel.isOuterVoxel)
            {
                Vector3 direction = (voxel.transform.Position - voxelMeshCenter.Position).normalized;
                float distanceFactor = Mathf.Clamp01((animationDistance - distanceToCamera) / animationDistance);
                voxel.AnimateAway(direction, distanceFactor * animationDistance, animationTime);
            }
            else
            {
                voxel.ResetPosition();
            }
        }
    }*/

    public void ClearData()
    {
        ComputeManager.Instance.ClearAndRequeueBuffer(data);
    }

    public void RenderMesh()
    {
        // USEFUL BUT SLOWS THINGS DOWN: Debug.Log("voxel generation happening!");
        SCManager.Instance.reRenderingMesh = true;
        meshData.ClearData();
        GenerateMesh();
        UploadMesh();
        SCManager.Instance.reRenderingMesh = false;     
    }

    public void ReRenderMesh()
    {
        RenderMesh();
        // USEFUL BUT SLOWS THINGS DOWN:
        Debug.Log("voxel generation done!");
    }

    Dictionary<Vector3Int, Voxel> visibleVoxels = null;

    public void GenerateMesh()
    {
        // FOR FUTURE // Adjust the near clipping distance based on target
        // FOR FUTURE nearClippingDistance = Mathf.Lerp(nearClippingDistance, targetNearClippingDistance, Time.deltaTime * adjustmentSpeed);


        int voxelsSelected = 0;

        int counter = 0;
        Vector3Int[] faceVertices = new Vector3Int[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        Dictionary<Vector3Int, Voxel> visibleVoxels = GetVisibleVoxels(mainCamera);
        // USEFUL BUT SLOWS THINGS DOWN: Debug.Log("About to render this many potential voxels: " + visibleVoxels.Count);

        // FIXP foreach (VoxelCell nextVoxel in WorldManager.Instance.voxelDictionary)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.voxelDictionary)
        foreach (var nextVoxel in visibleVoxels)
        {
            //voxelBlockPosition = new Vector3Int(nextVoxel.x, nextVoxel.y, nextVoxel.z);
            if (nextVoxel.Key.x < 0 || nextVoxel.Key.y < 0 || nextVoxel.Key.z < 0)
            {
                continue;
            }

            // Skip this voxel if it's below the visibility threshold
            //if (int.Parse(nextVoxel.Value.color.ToString()) <= WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold)
            //    continue;

            if (checkVoxelIsSolid(nextVoxel.Key, ref visibleVoxels) == false)
                continue;

            // original
            /*            block = this[voxelBlockPosition];
                        //Only check on solid blocks
                        if (!block.isSolid)
                        {
                            Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + nextVoxel.x + "," + nextVoxel.z + "," + nextVoxel.z + "]");
                            continue;
                        }*/


            float grayScaleValue = float.Parse(nextVoxel.Value.colorR.ToString())/255f;
            voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
            voxelColorAlpha.a = 1;
            voxelColorAlpha = voxelColor.color;
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

            if (nextVoxel.Value.isSegmentVoxel)
            {             
                CreateClickableVoxel(nextVoxel.Key);
            }

            //Iterate over each face direction
            for (int i = 0; i < 6; i++)
            {
                //Check if there's a solid block against this face
                //if (checkVoxelIsSolid(voxelBlockPosition + voxelFaceChecks[i]))
                //if ( float.Parse(nextVoxel.color) < 18)
                //    continue;

                //Draw this face

                //Collect the appropriate vertices from the default vertices and add the block Position
                for (int j = 0; j < 4; j++)
                {
                    //faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + voxelBlockPosition;
                    faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + nextVoxel.Key;
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
            voxelsSelected++;
        }
        WorldManager.Instance.voxelsSelected = voxelsSelected;
    }

    float maxDistance = 200f;

    // Experiment that didn't work out.
    /*public void AdaptMesh()
    {
        //Voxel block;

        int voxelsSelected = 0;

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        meshData.colors = new List<Color>();

        List<Voxel> visibleVoxels = GetVisibleVoxels(mainCamera);

        // FIXP foreach (VoxelCell nextVoxel in WorldManager.Instance.voxelDictionary)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.voxelDictionary)
        foreach (Voxel nextVoxel in visibleVoxels)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.voxelDictionary)
        {
            //voxelBlockPosition = new Vector3Int(nextVoxel.x, nextVoxel.y, nextVoxel.z);
            if (nextVoxel.x < 0 || nextVoxel.y < 0 || nextVoxel.z < 0)
            {
                continue;
            }

            // Skip this voxel if it's below the visibility threshold
            if (int.Parse(nextVoxel.color) <= WorldManager.Instance.voxelMeshConfigurationSettings.visibilityThreshold)
                continue;

            // original
            *//*            block = this[voxelBlockPosition];
                        //Only check on solid blocks
                        if (!block.isSolid)
                        {
                            Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + nextVoxel.x + "," + nextVoxel.z + "," + nextVoxel.z + "]");
                            continue;
                        }*//*



            float grayScaleValue = float.Parse(nextVoxel.color) / 255f;

*//*          THIS IS FOR LATER WHEN TRYING TO ADJUST TRANSPARENCY BASEd ON CAMERA LOCATION  
 *          float camDistance = Vector3.Distance(mainCamera.transform.Position, WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter);
            if (camDistance < 100f)
            {
                grayScaleValue = AdjustGrayscale(grayScaleValue, camDistance);
                voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
                voxelColorAlpha.a = camDistance / 50;
            }
            else
            {*//*
                voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
                voxelColorAlpha.a = 1;
            //}

            voxelColorAlpha = voxelColor.color;
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

            //Iterate over each face direction
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    // FOr this to be meaningful, we really need to track where each voxel is in the mesh so that we can
                    // change individual colours!!!
                    // THis will save looping through each voxel all the time!!
                    meshData.colors.Add(voxelColorAlpha);
                }
            }
            voxelsSelected++;
        }
        WorldManager.Instance.voxelsSelected = voxelsSelected;
    }*/

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
            ConfigureComponents();

        meshFilter.mesh = meshData.mesh;
/*        if (meshData.vertices.Count > 3)
            meshCollider.sharedMesh = meshData.mesh;
*/    }

    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
/*        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.convex = false;
            //meshCollider.cookingOptions &= ~MeshColliderCookingOptions.UseFastMidphase;
            //meshCollider.cookingOptions |= ~MeshColliderCookingOptions.CookForFasterSimulation;
        }*/
    }

    Vector3 chunkDimensions = new Vector3(WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize);

    public Dictionary<Vector3Int, Voxel> GetVisibleVoxels(Camera camera)
    {
        // Pre-calculate the Frustrum planes so that it isn't calculated each loop!
        FrustumCulling.CalculateFrustrumPlanes(camera);

        // This will hold the voxels that are ultimately doing to be visible.
        visibleVoxels = new Dictionary<Vector3Int, Voxel>(WorldManager.Instance.voxelDictionary.Count, new FastVector3IntComparer());

/*        foreach (var chunk in WorldManager.Instance.voxelChunks)
        {
            if (FrustumCulling.IsChunkInView(ref chunk.Value.bounds)) //camera, chunkPosition, chunkDimensions))
            {
                // Only check individual voxels within the chunk if the chunk is visible
                TraverseVisibleChunk(ref camera, chunk.Key, ref visibleVoxels);
            }
        }*/

        FrustumCulling.DropFrustrumPlanes();

        Traverse(camera, visibleVoxels);

        return visibleVoxels;

    }

    private void TraverseVisibleChunk(ref Camera camera, Vector3Int chunkPosition, ref Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        Chunk chunkValue = null;
        WorldManager.Instance.voxelChunks.TryGetValue(chunkPosition, out chunkValue);

        if (chunkValue != null)
        {
            foreach(var nextVoxelInChunk in chunkValue.voxels)
            {
                if (FrustumCulling.IsVoxelInView(camera, nextVoxelInChunk.Key, 100, WorldManager.Instance.worldSettings.nearClippingDistance))
                {
                    visibleVoxels.Add(nextVoxelInChunk.Key, nextVoxelInChunk.Value);
                }
            }
        }


/*        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {                    
                    Vector3Int voxelPosition = new Vector3Int(x, y, z) + Vector3Int.FloorToInt(chunkPosition);

                    if (FrustumCulling.IsVoxelInView(camera, voxelPosition, 100, WorldManager.Instance.worldSettings.nearClippingDistance))
                    {
                        visibleVoxels.Add(voxelPosition, WorldManager.Instance.voxelDictionary[voxelPosition]);
                    }
                }
            }
        }*/
    }

    private void Traverse(Camera camera, Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        foreach (var nextVoxel in WorldManager.Instance.voxelDictionary)
        {
            if (FrustumCulling.IsVoxelInView(camera, nextVoxel.Key, 8, WorldManager.Instance.worldSettings.nearClippingDistance))
            // D AS L if (FrustumCulling.IsVoxelInView(camera, Vector3IntConvertor.DecodeVector3Int(nextVoxel.Key), 4))
            {
                visibleVoxels.Add(nextVoxel.Key, nextVoxel.Value);
            }
        }
    }

    public bool checkVoxelIsSolid(Vector3Int voxelPosition, ref Dictionary<Vector3Int, Voxel> visibleVoxels)
    {
        foreach (var dir in directions)
        {
            if (!visibleVoxels.ContainsKey(voxelPosition + dir))
            {
                return true; // If any neighbor is missing, the voxel is visible
            }
        }
        return false;
/*        if (voxelPosition.y + 2 < 0 || (voxelPosition.x > WorldManager.WorldSettings.maxWidthX + 2) || (voxelPosition.z > WorldManager.WorldSettings.maxDepthZ + 2))
            return true;
        else
            // We can make use of an array of Vector locations, used as an index - need to update the NoiseBuffer class.
            return true; // Temp!!! this[voxelPosition].isSolid;*/
    }

    // We can make use of an array of Vector locations, used as an index - need to update the NoiseBuffer class.
/*    public VoxelOriginal this[int index]
    {
        get
        {
            return data[index];
        }

        set
        {
            data[index] = value;
        }
    }*/

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

                // Debug.Log("Upload: colors=" + colors.Count);
                // Debug.Log("Upload: vertices=" + vertices.Count);

                mesh.SetUVs(0, UVs);
                mesh.SetUVs(2, UVs2);

                mesh.Optimize();

                mesh.RecalculateNormals();

                mesh.RecalculateBounds();

                mesh.UploadMeshData(false);
            } catch (Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.Log("Error: colors="+colors.Count);
                Debug.Log("Error: vertices=" + vertices.Count);
            }
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

