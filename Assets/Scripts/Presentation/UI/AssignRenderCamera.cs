using UnityEngine;

[RequireComponent(typeof(Canvas))]
[ExecuteAlways]
public class AssignRenderCamera : MonoBehaviour
{
    public int targetSortingOrder = 0;
    private Canvas _canvas;

    void Start()
    {
        Setup();
    }

    void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        _canvas = GetComponent<Canvas>();
        if (gameObject.name == "BackgroundCanvas")
        {
            Debug.Log("AssignRenderCamera: Configuring BackgroundCanvas");
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.planeDistance = 50; // Closer but still behind 0
            targetSortingOrder = -1000;

            var scaler = GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            var image = transform.Find("Image");
            if (image != null)
            {
                var rect = image.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    rect.localPosition = Vector3.zero;
                    rect.localScale = Vector3.one;
                }
            }
        }
        else if (gameObject.name == "Canvas")
        {
             // HUD Canvas should be on top
             targetSortingOrder = 100;
        }

        AssignCamera();
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        
        if (_canvas.worldCamera == null)
        {
            AssignCamera();
        }
    }

    private void AssignCamera()
    {
        var mainCam = Camera.main;
        if (mainCam == null)
        {
            // Fallback: Try finding by name if tag lookup fails
            var camObj = GameObject.Find("Main Camera");
            if (camObj != null) mainCam = camObj.GetComponent<Camera>();
        }

        if (mainCam != null)
        {
            Debug.Log($"AssignRenderCamera: Found camera {mainCam.name}. Assigning to canvas {_canvas.name}");
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = mainCam;
            _canvas.planeDistance = 50;
            _canvas.sortingOrder = targetSortingOrder;
        }
    }
}
