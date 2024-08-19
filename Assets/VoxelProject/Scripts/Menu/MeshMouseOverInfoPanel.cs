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

    string brainText, liverText, heartText = null;

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
                        if (heartText == null)
                        {
                            loadingData = true;
                            heartText = WorldManager.Instance.menuHandler.LoadHeartFileHeader();
                            heartText = "Heart!<br>" + heartText;
                        }
                        popupText.text = heartText;
                        popupText.color = Color.red;

                        // Show the panel
                        popupPanel.SetActive(true);                      
                    }
                    else if (hit.collider.gameObject.CompareTag("Liver"))
                    {
                        if (liverText == null)
                        {
                            loadingData = true;
                            liverText = WorldManager.Instance.menuHandler.LoadLiverFileHeader();
                            liverText = "Liver!<br>" + liverText;
                        }
                        popupText.text = liverText;
                        popupText.color = Color.yellow;

                        // Show the panel
                        popupPanel.SetActive(true);
                    }
                    else if (hit.collider.gameObject.CompareTag("Brain"))
                    {
                        if (brainText == null)
                        {
                            loadingData = true;
                            brainText = WorldManager.Instance.menuHandler.LoadBrainFileHeader();
                            brainText = "Brain!<br>" + brainText;
                        }
                        popupText.text = brainText;
                        popupText.color = Color.cyan;

                        // Show the panel
                        popupPanel.SetActive(true);
                    }

                    // Optionally, you can worldPosition the panel at the mouse worldPosition
                    //Vector3 mousePos = Input.mousePosition;
                    //popupPanel.transform.worldPosition = mousePos + new Vector3(popupPanel.GetComponent<RectTransform>().rect.width / 2, popupPanel.GetComponent<RectTransform>().rect.height / 2, 0);

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