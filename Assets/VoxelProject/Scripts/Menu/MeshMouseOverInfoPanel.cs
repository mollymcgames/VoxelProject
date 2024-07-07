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

    public TextMeshProUGUI popupText;

    private void Start()
    {
        // Initially hide the panel
        //popupPanel.SetActive(false);
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
                    popupText.text = "\nThis is all about the heart!\n";
                    // Show the panel
                    popupPanel.SetActive(true);
                }
                else if (hit.collider.gameObject.CompareTag("Liver"))
                {
                    popupText.text = "\nThis is all about the liver!\n";
                    // Show the panel
                    popupPanel.SetActive(true);
                }
                else if (hit.collider.gameObject.CompareTag("Brain"))
                {
                    popupText.text = "\nThis is all about the brain!\n";
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
                Debug.Log("Killing heart");
                // Hide the panel after the delay
                popupPanel.SetActive(false);
                hideTimer = 0f;
            }
        }
    }
}
