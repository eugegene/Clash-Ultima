using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(UnitStats))]
public class UnitMotor : MonoBehaviour
{
    private NavMeshAgent _agent;
    private UnitStats _stats;
    private bool _isSelected = true; // Later this will be handled by a SelectionManager

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<UnitStats>();
        
        // Disable Agent rotation so we can smooth it ourselves
        _agent.updateRotation = false; 
    }

    void OnEnable()
    {
        // Subscribe to the Input Event
        InputHandler.OnMoveCommand += OnMoveCommandReceived;
    }

    void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks!
        InputHandler.OnMoveCommand -= OnMoveCommandReceived;
    }

    void Update()
    {
        // 1. Update Speed from Stats (In case we got Buffed/Slowed this frame)
        // Note: In a super optimized game we would use events for speed changes too, 
        // but for 200 units, polling is safer for a beginner architecture.
        _agent.speed = _stats.MoveSpeed.Value;
        _agent.acceleration = 60f; // High acceleration for snappy MOBA feel

        // 2. Handle Rotation manually for snappy turns
        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _stats.definition.rotationSpeed);
        }
    }

    private void OnMoveCommandReceived(Vector3 destination)
    {
        // Only move if this specific unit is selected (For now, we assume single character)
        if (_isSelected)
        {
            _agent.SetDestination(destination);
        }
    }
}