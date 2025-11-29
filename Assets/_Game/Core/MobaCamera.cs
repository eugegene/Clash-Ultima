using UnityEngine;
using UnityEngine.InputSystem;

public class MobaCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform targetToFollow;
    public bool startLocked = true;

    [Header("Settings")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit = new Vector2(50, 50);

    [Header("Zoom")]
    public float scrollSpeed = 200f;
    public float minY = 10f;
    public float maxY = 40f;

    [Header("Smoothing")]
    public float smoothSpeed = 10f;

    private Vector3 _targetPosition;
    private bool _isLockedOnHero;

    void Start()
    {
        _isLockedOnHero = startLocked;
        _targetPosition = transform.position;
    }

    void Update()
    {
        // Toggle Lock
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _isLockedOnHero = !_isLockedOnHero;
        }

        Vector3 edgeMove = GetEdgeInput();

        // Logic: Edge Pan > Follow Hero
        if (edgeMove != Vector3.zero)
        {
            _isLockedOnHero = false;
            _targetPosition += edgeMove * panSpeed * Time.deltaTime;
        }
        else if (_isLockedOnHero && targetToFollow != null)
        {
            float currentHeight = _targetPosition.y; 
            _targetPosition = targetToFollow.position;
            _targetPosition.z -= 10f; // Offset
            _targetPosition.y = currentHeight; 
        }

        // Zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        _targetPosition.y -= scroll * scrollSpeed * 0.01f * Time.deltaTime; 
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, minY, maxY);

        // Clamp to Map
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -panLimit.x, panLimit.x);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, -panLimit.y, panLimit.y);

        // Smooth Move
        transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothSpeed * Time.deltaTime);
    }

    private Vector3 GetEdgeInput()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 move = Vector3.zero;

        if (mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height) 
            return Vector3.zero;

        if (mousePos.x >= Screen.width - panBorderThickness) move.x += 1;
        if (mousePos.x <= panBorderThickness) move.x -= 1;
        if (mousePos.y >= Screen.height - panBorderThickness) move.z += 1;
        if (mousePos.y <= panBorderThickness) move.z -= 1;

        return move;
    }
}