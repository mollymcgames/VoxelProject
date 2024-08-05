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
    //private Process currentProcess;

    void Start()
    {
        // Initialise variables
        timeSinceLastUpdate = 0.0f;
        frames = 0;
        fps = 0.0f;
        //currentProcess = Process.GetCurrentProcess();
        //UnityEngine.Debug.Log("CP: "+currentProcess.ToString());
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
            else
            {
                // Optionally, log FPS to console
                UnityEngine.Debug.Log("FPS: " + Mathf.RoundToInt(fps));
            }

            // Reset the counters
            frames = 0;
            timeSinceLastUpdate = 0.0f;
        }

        // DISPLAY RAM USAGE
        // Convert memory usage from bytes to megabytes
        float memoryUsageMB = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
        goTotalRAM.text = memoryUsageMB.ToString("F0") + "MB";

        // meshFilter = GameObject.FindAnyObjectByType<MeshFilter>();
        //find mesh instead
        mesh = GameObject.FindFirstObjectByType<Mesh>();
        // if (meshFilter != null)
        
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
                goChunksSelected.text = "NA"; // WorldManager.Instance.voxelGrid.chunks.Count.ToString();
                //goVoxelsSelected.text = WorldManager.Instance.voxelGrid.voxelsRepresented.ToString();
                goVoxelsSelected.text = WorldManager.Instance.voxelsSelected.ToString();
            } catch
            {
                goChunksSelected.text = "None";
                goVoxelsSelected.text = "None";
            }
        }
    
    }
}
