using UnityEngine;

public class CustomCursorHandler : MonoBehaviour
{
    public Texture2D customCursor; // The custom cursor texture
    public string targetTag; // The tag to detect

    private Texture2D defaultCursor; // To store the default cursor texture

    void Start()
    {
        // Store the default cursor texture (if you need to revert to it)
        defaultCursor = CursorTexture();
        customCursor = Resources.Load<Texture2D>("cursor-pointer");
    }

    void Update()
    {
        // Check for the object under the mouse cursor
        CheckForHover();
    }

    void OnMouseExit()
    {
        RestoreDefaultCursor();
    }

    public void RestoreDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseOver()
    {
        CheckForHover();
    }

    void CheckForHover()
    {
        // Raycast from the mouse worldPosition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit has the specified tag
            if (hit.collider.CompareTag(targetTag))
            {
                // Change the cursor to the custom cursor
                Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    Texture2D CursorTexture()
    {
        // This method gets the current cursor texture.
        // Note: Unity does not provide a direct way to get the current cursor texture.
        // You need to store the default cursor texture yourself, as done in the Start method.
        // Return null or create a default cursor texture if needed.
        return null;
    }
}
