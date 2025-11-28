using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    // We use C# Actions (Events) so other scripts can listen
    public static event Action<Vector3> OnMoveCommand;

    [Header("Settings")]
    public LayerMask groundLayer;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        // Check Right Mouse Button (New Input System)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // Raycast to find the ground position
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // Shout: "HEY! The player wants to move to THIS point!"
            OnMoveCommand?.Invoke(hit.point);
        }
    }
}