using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public Texture2D defaultCursor;
    public Texture2D hoverCursor;  
    public Vector2 defaultHotspot = Vector2.zero; 
    public Vector2 hoverHotspot = Vector2.zero;

    private void Awake()
    {
        SetDefaultCursor();
    }

    /// <summary>
    /// Sets the cursor to the default texture.
    /// </summary>
    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
    }

    /// <summary>
    /// Sets the cursor to the hover texture.
    /// </summary>
    public void SetHoverCursor()
    {
        Cursor.SetCursor(hoverCursor, hoverHotspot, CursorMode.Auto);
    }
}
