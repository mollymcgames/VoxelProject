using TMPro;
using UnityEngine;

// This class is responsible for displaying everything on the stats panel
public class MeshMemoryUsage : MonoBehaviour
{
    // UI Text objects to display the memory usage
    //Memory stats for different components of the mesh
    public TextMeshProUGUI goVertexMemory; //TMP Vertex memory usage
    public TextMeshProUGUI goNormalMemory; //TMP Normal memory usage
    public TextMeshProUGUI goUvMemory; //TMP UV memory usage
    public TextMeshProUGUI goIndexMemory; //TMP Index memory usage
    public TextMeshProUGUI goTotalMemory; //TMP Total memory usage
    public TextMeshProUGUI goChunksSelected; //TMP How many chunks are selected
    public TextMeshProUGUI goVoxelsSelected; //TMP How many voxels are selected to be displayed

    public TextMeshProUGUI goFPS; //UI element for the FPS
    public TextMeshProUGUI goTotalRAM;  //UI element for the total RAM usage

    // Variables to store the FPS calculation
    public float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0.0f;
    private int frames = 0; // Frames since last update
    private float fps = 0.0f; // Current FPS
    private Mesh mesh;

    private int vertexMemory = 0; // Memory used by vertices
    private int normalMemory = 0; // Memory used by normals
    private int uvMemory = 0; // Memory used by UVs
    private int indexMemory = 0; // Memory used by indices
    private int totalMemory = 0; // Total memory used by the mesh

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
        timeSinceLastUpdate += Time.deltaTime; // Accumulate time since last update for FPS calculation
        
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

            // Reset the frame counter and time for the next interval
            frames = 0;
            timeSinceLastUpdate = 0.0f;
        }

        // DISPLAY RAM USAGE
        // Convert memory usage from bytes to megabytes
        float memoryUsageMB = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
        //Display the memory usage in the UI in MB
        goTotalRAM.text = memoryUsageMB.ToString("F0") + "MB"; //F0 converts to a whole number

        mesh = GameObject.FindFirstObjectByType<Mesh>(); // Find the first object with a mesh
        
        if (mesh != null)
        {
            // Calculate the memory used by the vertices (each vertex has 3 floats)
            vertexMemory = mesh.vertexCount * sizeof(float) * 3;
            // Calculate the memory used by the normals (each normal has 3 floats)
            normalMemory = mesh.normals.Length * sizeof(float) * 3;
            // Calculate the memory used by the UVs (each UV has 2 floats)
            uvMemory = mesh.uv.Length * sizeof(float) * 2;
            // Calculate the memory used by the indices (each index is an integer)
            indexMemory = mesh.triangles.Length * sizeof(int); 
            // Calculate the total memory usage by summing each component
            totalMemory = vertexMemory + normalMemory + uvMemory + indexMemory;

            //Display the memory usage in the UI in MB (by converting bytes to MB)
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
                //If the WorldManager is not found, display "None" in the UI
                goChunksSelected.text = "None";
                goVoxelsSelected.text = "None";
            }
        }
    }
}
