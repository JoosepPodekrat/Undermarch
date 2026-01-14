using UnityEngine;
using UnityEngine.InputSystem;

namespace Undermarch.Presentation.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom Settings")]
        public float minZoom = 3f;
        public float maxZoom = 15f;
        public float zoomSpeed = 1f;

        [Header("Pan Settings")]
        public float panSpeed = 1f; // Multiplier for drag speed

        [Header("Board Bounds")]
        public int boardWidth = 20;  // Board width in tiles
        public int boardHeight = 20; // Board height in tiles
        public float boundsMargin = 2f; // Extra margin beyond board edge

        private Camera _cam;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;
        private bool _hasLoggedMove = false; // To prevent spam
        private InputSystem_Actions _actions;
        private Vector2 _moveInput;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
            Debug.Log("CameraController: Active and waiting for Camera...");
        }

        private void OnEnable()
        {
            _actions.Player.Move.performed += OnMove;
            _actions.Player.Move.canceled += OnMove;
            _actions.Player.Enable();
        }

        private void OnDisable()
        {
            _actions.Player.Move.performed -= OnMove;
            _actions.Player.Move.canceled -= OnMove;
            _actions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        
        // Using LateUpdate to ensure we have the last say on the camera's position for the frame.
        private void LateUpdate()
        {
            if (_cam == null)
            {
                _cam = Camera.main;
                if (_cam != null)
                {
                    Debug.Log($"CameraController: Connected to camera '{_cam.name}'");
                }
                else
                {
                    return; // No camera found yet, try again next frame.
                }
            }

            Vector3 posBefore = _cam.transform.position;
            float sizeBefore = _cam.orthographicSize;

            HandleZoom();
            HandlePan();
            ClampCameraPosition();

            if (!_hasLoggedMove && (posBefore != _cam.transform.position || sizeBefore != _cam.orthographicSize))
            {
                Debug.Log($"CameraController: Camera position/zoom has been successfully modified. From Pos/Size: {posBefore}/{sizeBefore} To: {_cam.transform.position}/{_cam.orthographicSize}");
                _hasLoggedMove = true; // Log only once to prevent spam
            }
        }

        private void HandleZoom()
        {
            // New Input System
            float scroll = Mouse.current != null ? Mouse.current.scroll.y.ReadValue() : 0;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float newSize = _cam.orthographicSize - (scroll * zoomSpeed * 0.25f);
                _cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }

        private void HandlePan()
        {
            // Pan with keyboard
            var move = new Vector3(_moveInput.x, _moveInput.y, 0);
            transform.Translate(move * (panSpeed * 10) * Time.deltaTime, Space.World);
            
            // Use isPressed for continuous state, more reliable for dragging
            bool rightButtonIsPressed = (Mouse.current != null && Mouse.current.rightButton.isPressed);
            
            if (rightButtonIsPressed)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector3 currentPos = _cam.ScreenToWorldPoint(mousePosition);

                // Check if this is the first frame of the drag
                if (!_isDragging)
                {
                    _isDragging = true;
                    _dragOrigin = currentPos; // Set the origin point
                    Debug.Log("PAN START: Drag initiated at world pos: " + _dragOrigin);
                }
                else
                {
                    // We are in the middle of a drag
                    Vector3 difference = _dragOrigin - currentPos;
                    if (difference.magnitude > 0.001f) // Threshold to prevent micro-drifts
                    {
                        // No log here to prevent spam, the move itself is the proof
                        _cam.transform.position += difference;
                    }
                }
            }
            else
            {
                // Check if we just released the button
                if (_isDragging)
                {
                    Debug.Log("PAN END: Drag finished.");
                    _isDragging = false;
                }
            }
        }

        private void ClampCameraPosition()
        {
            if (_cam == null) return;

            // Get the visible bounds based on camera's orthographic size
            float verticalSize = _cam.orthographicSize;
            float horizontalSize = verticalSize * _cam.aspect;

            // Calculate min/max bounds (board is centered at 0,0,0)
            // Board extends from -width/2 to width/2, -height/2 to height/2
            float minX = -(boardWidth / 2f) - boundsMargin + horizontalSize;
            float maxX = (boardWidth / 2f) + boundsMargin - horizontalSize;
            float minY = -(boardHeight / 2f) - boundsMargin + verticalSize;
            float maxY = (boardHeight / 2f) + boundsMargin - verticalSize;

            // Ensure min < max (for very small zoom levels)
            if (minX > maxX) minX = maxX = (minX + maxX) / 2f;
            if (minY > maxY) minY = maxY = (minY + maxY) / 2f;

            // Clamp position
            Vector3 pos = _cam.transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            _cam.transform.position = pos;
        }
    }
}
