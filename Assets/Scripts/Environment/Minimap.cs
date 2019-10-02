using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam = null;

    private const int MINIMAP_PIXEL_WIDTH = 200;
    private const float MARGIN = 0.1f;

    void Start()
    {
        // Automatically calculates where to position the minimap based on screen size and resolution
        float width = (float)MINIMAP_PIXEL_WIDTH / (float)_mainCam.pixelWidth;
        float height = (float)MINIMAP_PIXEL_WIDTH / (float)_mainCam.pixelHeight;
        GetComponent<Camera>().rect = new Rect(width * MARGIN, 1 - (MARGIN + 1) * height, width, height);
    }
}
