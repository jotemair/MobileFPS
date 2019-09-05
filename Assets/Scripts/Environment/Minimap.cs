using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam = null;

    private const int MINIMAP_PIXEL_WIDTH = 200;
    private const float MARGIN = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        float width = (float)MINIMAP_PIXEL_WIDTH / (float)_mainCam.pixelWidth;
        float height = (float)MINIMAP_PIXEL_WIDTH / (float)_mainCam.pixelHeight;
        GetComponent<Camera>().rect = new Rect(width * MARGIN, 1 - (MARGIN + 1) * height, width, height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
