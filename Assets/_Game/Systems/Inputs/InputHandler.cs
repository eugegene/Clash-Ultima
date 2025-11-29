using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    public static event Action<Vector3> OnMoveCommand;
    // New Event for Attacking
    public static event Action<UnitStats> OnAttackCommand;

    [Header("Settings")]
    public LayerMask groundLayer;
    public LayerMask unitLayer; // Assign this in Inspector! (Usually "Default" or a custom "Unit" layer)

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleRightClick();
        }
    }

    private void HandleRightClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // 1. Check if we clicked a Unit
        if (Physics.Raycast(ray, out hit, 100f, unitLayer))
        {
            UnitStats target = hit.collider.GetComponent<UnitStats>();
            if (target != null)
            {
                OnAttackCommand?.Invoke(target);
                return; // Priority: Unit Click > Ground Click
            }
        }

        // 2. Check if we clicked Ground
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            OnMoveCommand?.Invoke(hit.point);
        }
    }
}