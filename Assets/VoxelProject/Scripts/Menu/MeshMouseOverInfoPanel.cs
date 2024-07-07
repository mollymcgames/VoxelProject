using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeshMouseOverInfoPanel : MonoBehaviour
{
    public GameObject popupPanel;  // Reference to the panel GameObject
    public LayerMask layerMask;    // Layer mask to specify which layers to interact with
    private bool isMouseOver = false;
    private float hideDelay = 1.1f; // Time to wait before hiding the panel
    private float hideTimer = 0f;

    MenuHandler menuHandler;
    public TextMeshProUGUI popupText;

    string brainText, liverText, heartText = null;

    private void Start()
    {
        menuHandler = new MenuHandler();
        // Initially hide the panel
        popupPanel.SetActive(false);
    }

    void Update()
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
                        heartText = menuHandler.LoadHeartFileHeader();
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
                        liverText = menuHandler.LoadLiverFileHeader();
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
                        brainText = menuHandler.LoadBrainFileHeader();
                        brainText = "Brain!<br>" + brainText;
                    }
                    popupText.text = brainText;
                    popupText.color = Color.cyan;

                    // Show the panel
                    popupPanel.SetActive(true);
                }

                // Optionally, you can position the panel at the mouse position
                //Vector3 mousePos = Input.mousePosition;
                //popupPanel.transform.position = mousePos + new Vector3(popupPanel.GetComponent<RectTransform>().rect.width / 2, popupPanel.GetComponent<RectTransform>().rect.height / 2, 0);

                // Set isMouseOver to true and reset the hide timer
                isMouseOver = true;
                hideTimer = 0f;
            }
            else
            {
                // Set isMouseOver to false and start the hide timer
                isMouseOver = false;
            }
        }
        else
        {
            // Set isMouseOver to false and start the hide timer
            isMouseOver = false;
        }

        // If the mouse is not over the mesh, start the hide timer
        if (!isMouseOver)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideDelay)
            {
                // Hide the panel after the delay
                popupPanel.SetActive(false);
                hideTimer = 0f;
            }
        }
    }
}
