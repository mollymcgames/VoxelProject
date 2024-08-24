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
    public TextMeshProUGUI goTotalRAM;

    // Variables to store the FPS calculation
    public float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0.0f;
    private int frames = 0;
    private float fps = 0.0f;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private int vertexMemory = 0;
    private int normalMemory = 0;
    private int uvMemory = 0;
    private int indexMemory = 0;
    private int totalMemory = 0;

    void Start()
    {
        // Initialise variables
        timeSinceLastUpdate = 0.0f;
        frames = 0;
        fps = 0.0f;
    }

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

            // Reset the counters
            frames = 0;
            timeSinceLastUpdate = 0.0f;
        }

        // DISPLAY RAM USAGE
        // Convert memory usage from bytes to megabytes
        float memoryUsageMB = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
        goTotalRAM.text = memoryUsageMB.ToString("F0") + "MB";

        mesh = GameObject.FindFirstObjectByType<Mesh>();
        
        if (mesh != null)
        {
            vertexMemory = mesh.vertexCount * sizeof(float) * 3; // Each vertex has 3 floats (x, y, z)
            normalMemory = mesh.normals.Length * sizeof(float) * 3; // Each normal has 3 floats
            uvMemory = mesh.uv.Length * sizeof(float) * 2; // Each UV has 2 floats (u, v)
            indexMemory = mesh.triangles.Length * sizeof(int); // Each index is an int
            totalMemory = vertexMemory + normalMemory + uvMemory + indexMemory;

            goVertexMemory.text = (vertexMemory / 1024f / 1024f).ToString() + " MB";
            goNormalMemory.text = (normalMemory / 1024f / 1024f).ToString() + " MB";
            goUvMemory.text = (uvMemory / 1024f / 1024f).ToString() + " MB";
            goIndexMemory.text = (indexMemory / 1024f / 1024f).ToString() + " MB";
            goTotalMemory.text = (totalMemory / 1024f / 1024f).ToString() + " MB";

            try
            {
                goChunksSelected.text = WorldManager.Instance.chunksOnDisplay.ToString();
                goVoxelsSelected.text = WorldManager.Instance.voxelsSelected.ToString();
            } 
            catch
            {
                goChunksSelected.text = "None";
                goVoxelsSelected.text = "None";
            }
        }
    }
}
