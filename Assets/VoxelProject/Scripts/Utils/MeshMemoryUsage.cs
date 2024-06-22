using TMPro;
using UnityEngine;

public class MeshMemoryUsage : MonoBehaviour
{
    public TextMeshProUGUI goVertexMemory;
    public TextMeshProUGUI goNormalMemory;
    public TextMeshProUGUI goUvMemory;
    public TextMeshProUGUI goIndexMemory;
    public TextMeshProUGUI goTotalMemory;
    public TextMeshProUGUI goChunksSelected;
    public TextMeshProUGUI goVoxelsSelected;

    public TextMeshProUGUI goFPS;

    // Variables to store the FPS calculation
    public float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0.0f;
    private int frames = 0;
    private float fps = 0.0f;

    void Start()
    {
        // Initialise variables
        timeSinceLastUpdate = 0.0f;
        frames = 0;
        fps = 0.0f;
    }


/*    // Optionally, add an OnGUI method to display FPS on screen without UI text
    void OnGUI()
    {
        // Display FPS as overlay text on screen
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + Mathf.RoundToInt(fps));
    }*/

    void Update()
    {
        // Increment frame count
        frames++;
        timeSinceLastUpdate += Time.deltaTime;
        
        // Check if the time since last update interval has passed
        if (timeSinceLastUpdate >= updateInterval)
        {
            // Calculate FPS
            fps = frames / timeSinceLastUpdate;

            // Update the UI text if assigned
            if (goFPS != null)
            {
                goFPS.text = Mathf.RoundToInt(fps).ToString();
            }
            else
            {
                // Optionally, log FPS to console
                Debug.Log("FPS: " + Mathf.RoundToInt(fps));
            }

            // Reset the counters
            frames = 0;
            timeSinceLastUpdate = 0.0f;
        }

        MeshFilter meshFilter = GameObject.FindAnyObjectByType<MeshFilter>();
        if (meshFilter != null)
        {
            //        Mesh mesh = GetComponent<MeshFilter>().mesh;
            Mesh mesh = meshFilter.mesh;
            if (mesh != null)
            {
                int vertexMemory = mesh.vertexCount * sizeof(float) * 3; // Each vertex has 3 floats (x, y, z)
                int normalMemory = mesh.normals.Length * sizeof(float) * 3; // Each normal has 3 floats
                int uvMemory = mesh.uv.Length * sizeof(float) * 2; // Each UV has 2 floats (u, v)
                int indexMemory = mesh.triangles.Length * sizeof(int); // Each index is an int

                int totalMemory = vertexMemory + normalMemory + uvMemory + indexMemory;

                //Debug.Log($"Mesh Memory Usage: {totalMemory / 1024f / 1024f} MB");
                goVertexMemory.text = (vertexMemory / 1024f / 1024f).ToString() + " MB";
                goNormalMemory.text = (normalMemory / 1024f / 1024f).ToString() + " MB";
                goUvMemory.text = (uvMemory / 1024f / 1024f).ToString() + " MB";
                goIndexMemory.text = (indexMemory / 1024f / 1024f).ToString() + " MB";
                goTotalMemory.text = (totalMemory / 1024f / 1024f).ToString() + " MB";

                try
                {
                    goChunksSelected.text = VoxelWorldManager.Instance.voxelMeshContainer.renderVectors.Count.ToString();
                    goVoxelsSelected.text = VoxelWorldManager.Instance.voxelMeshContainer.voxelsInChunks.ToString();
                } catch
                {
                    goChunksSelected.text = "None";
                    goVoxelsSelected.text = "None";
                }
            }
        }
    }
}