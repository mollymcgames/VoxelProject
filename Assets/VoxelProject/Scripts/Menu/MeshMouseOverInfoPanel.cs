using UnityEngine;
using TMPro;

public class MeshMouseOverInfoPanel : MonoBehaviour
{
    // Reference to the panel GameObjects
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    // Layer mask to specify which layers to interact with
    public LayerMask layerMask;

    // Time to wait before hiding the panel
    private float hideDelay = 1.5f;
    private float hideTimer = 0f;

    private bool hasMouseExited = false;
    public bool loadingData = false;

    string brainText, liverText, heartText, spineText = null;

    private void Start()
    {
        // Initially hide the panel
        popupPanel.SetActive(false);        
    }

    void OnMouseExit()
    {
        if (gameObject.CompareTag("Heart"))
        {
            // Set the boolean to true if the tag is "Heart"
            hasMouseExited = true;
        }
        if (gameObject.CompareTag("Liver"))
        {
            // Set the boolean to true if the tag is "Liver"
            hasMouseExited = true;
        }
        if (gameObject.CompareTag("Brain"))
        {
            // Set the boolean to true if the tag is "Brain"
            hasMouseExited = true;
        }
        if (gameObject.CompareTag("Spine"))
        {
            // Set the boolean to true if the tag is "Spine"
            hasMouseExited = true;
        }
    }

    void Update()
    {
        if (loadingData == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Check if the hit object is the specific mesh
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.CompareTag("Heart"))
                    {
                        popupText.text = "Loading Heart data, please wait...";

                        // Show the panel
                        popupPanel.SetActive(true);

                        if (heartText == null)
                        {
                            loadingData = true;
                            heartText = WorldManager.Instance.menuHandler.LoadHeartFile();
                            heartText = "Heart!<br>" + heartText;
                        }
                        popupText.text = heartText;                        
                    }
                    else if (hit.collider.gameObject.CompareTag("Liver"))
                    {
                        popupText.text = "Loading Liver data, please wait...";

                        // Show the panel
                        popupPanel.SetActive(true);

                        if (liverText == null)
                        {
                            loadingData = true;
                            liverText = WorldManager.Instance.menuHandler.LoadLiverFile();
                            liverText = "Liver!<br>" + liverText;
                        }
                        popupText.text = liverText;
                    }
                    else if (hit.collider.gameObject.CompareTag("Brain"))
                    {
                        popupText.text = "Loading Brain data, please wait...";

                        // Show the panel
                        popupPanel.SetActive(true);

                        if (brainText == null)
                        {
                            loadingData = true;
                            brainText = WorldManager.Instance.menuHandler.LoadBrainFile();
                            brainText = "Brain!<br>" + brainText;
                        }
                        popupText.text = brainText;
                    }
                    else if (hit.collider.gameObject.CompareTag("Spine"))
                    {
                        popupText.text = "Loading Spine data, please wait...";

                        // Show the panel
                        popupPanel.SetActive(true);

                        if (spineText == null)
                        {
                            loadingData = true;
                            spineText = WorldManager.Instance.menuHandler.LoadSpineFile();
                            spineText = "Spine!<br>" + spineText;
                        }
                        popupText.text = spineText;
                    }

                    // Set isMouseOver to true and reset the hide timer
                    hideTimer = 0f;
                }
            }
        }        

        // If the mouse is not over the mesh, start the hide timer
        if (hasMouseExited == true)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideDelay)
            {
                // Hide the panel after the delay
                popupPanel.SetActive(false);
                hideTimer = 0f;
                hasMouseExited = false;
                loadingData = false;
            }
        }
    }
}