using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(UnitStats))]
public class UnitMotor : MonoBehaviour
{
    private NavMeshAgent _agent;
    private UnitStats _stats;
    private Vector3 _lastDestination;
    public bool IsSelected = true; // Made public for debug

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<UnitStats>();
        _agent.updateRotation = false; 
    }

    void OnEnable()
    {
        InputHandler.OnMoveCommand += OnMoveCommandReceived;
    }

    void OnDisable()
    {
        InputHandler.OnMoveCommand -= OnMoveCommandReceived;
    }

    void Update()
    {
        _agent.speed = _stats.MoveSpeed.Value;
        
        // Manual Rotation
        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _stats.definition.rotationSpeed);
        }
    }

    // --- NEW PUBLIC METHODS ---

    public void MoveToPoint(Vector3 point)
    {
        _agent.isStopped = false;

        // Optimization: Only calculate path if the target moved more than 0.1m
        if (Vector3.Distance(point, _lastDestination) > 0.1f)
        {
            _agent.SetDestination(point);
            _lastDestination = point;
        }
    }

    public void StopMoving()
    {
        if (_agent.isOnNavMesh) 
            _agent.isStopped = true;
    }

    // --------------------------

    private void OnMoveCommandReceived(Vector3 destination)
    {
        if (IsSelected)
        {
            MoveToPoint(destination);
        }
    }
}