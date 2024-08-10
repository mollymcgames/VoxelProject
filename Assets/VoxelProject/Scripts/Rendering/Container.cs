using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Container : MonoBehaviour
{
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
        foreach (VoxelCell voxel in WorldManager.Instance.sourceData)
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

    public static List<VoxelCell> GetVisibleVoxels(Camera camera)
    {
        List<VoxelCell> visibleVoxels = new List<VoxelCell>();
        Traverse(camera, visibleVoxels);
        return visibleVoxels;
    }

    private static void Traverse(Camera camera, List<VoxelCell> visibleVoxels)
    {
        foreach (var nextVoxel in WorldManager.Instance.sourceData)
        {
            if (FrustumCulling.IsVoxelInView(camera, nextVoxel.Value.Position, 4))
            {
                visibleVoxels.Add(nextVoxel.Value);
            }
        }
    }

    public void GenerateMesh()
    {
        int voxelsSelected = 0;

        //Vector3Int voxelBlockPosition;

        int counter = 0;
        Vector3Int[] faceVertices = new Vector3Int[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        List<VoxelCell> visibleVoxels = GetVisibleVoxels(mainCamera);

        // FIXP foreach (VoxelCell nextVoxel in WorldManager.Instance.sourceData)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.sourceData)
        foreach (VoxelCell nextVoxel in visibleVoxels)
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
            /*            block = this[voxelBlockPosition];
                        //Only check on solid blocks
                        if (!block.isSolid)
                        {
                            Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + nextVoxel.x + "," + nextVoxel.z + "," + nextVoxel.z + "]");
                            continue;
                        }*/



            float grayScaleValue = float.Parse(nextVoxel.color)/255f;
            voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
            voxelColorAlpha.a = 1;
            voxelColorAlpha = voxelColor.color;
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

                //Collect the appropriate vertices from the default vertices and add the block Position
                for (int j = 0; j < 4; j++)
                {
                    //faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + voxelBlockPosition;
                    faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + nextVoxel.Position;
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

    public void AdaptMesh()
    {
        //Voxel block;

        int voxelsSelected = 0;

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        meshData.colors = new List<Color>();

        List<VoxelCell> visibleVoxels = GetVisibleVoxels(mainCamera);

        // FIXP foreach (VoxelCell nextVoxel in WorldManager.Instance.sourceData)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.sourceData)
        foreach (VoxelCell nextVoxel in visibleVoxels)
        // DICTIONARY ONLY foreach (var nextVoxel in WorldManager.Instance.sourceData)
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
            /*            block = this[voxelBlockPosition];
                        //Only check on solid blocks
                        if (!block.isSolid)
                        {
                            Debug.Log("Non solid block encountered (Loop-"+breaker+")! [" + nextVoxel.x + "," + nextVoxel.z + "," + nextVoxel.z + "]");
                            continue;
                        }*/



            float grayScaleValue = float.Parse(nextVoxel.color) / 255f;

/*          THIS IS FOR LATER WHEN TRYING TO ADJUST TRANSPARENCY BASEd ON CAMERA LOCATION  
 *          float camDistance = Vector3.Distance(mainCamera.transform.Position, WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter);
            if (camDistance < 100f)
            {
                grayScaleValue = AdjustGrayscale(grayScaleValue, camDistance);
                voxelColor = new VoxelColor(grayScaleValue, grayScaleValue, grayScaleValue);
                voxelColorAlpha.a = camDistance / 50;
            }
            else
            {*/
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

