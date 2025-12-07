using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitStats))]
public class UnitRangeDisplay : MonoBehaviour
{
    private UnitStats _stats;
    private GameObject _rangeCircle;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        CreateRangeCircle();
    }

    void CreateRangeCircle()
    {
        // Simple cylinder primitive
        _rangeCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _rangeCircle.name = "AttackRangeIndicator";
        
        // Remove collider so it doesn't block clicks
        Destroy(_rangeCircle.GetComponent<Collider>());
        
        // Make it a child so it follows us
        _rangeCircle.transform.SetParent(transform);
        _rangeCircle.transform.localPosition = Vector3.zero;
        
        // Flatten it
        _rangeCircle.transform.localScale = new Vector3(1, 0.05f, 1);
        
        // Optional: Set a transparent material if you have one
        // _rangeCircle.GetComponent<Renderer>().material = ...
        
        _rangeCircle.SetActive(false);
    }

    void Update()
    {
        // Check for Left Alt
        if (Keyboard.current.leftAltKey.isPressed)
        {
            if (!_rangeCircle.activeSelf) _rangeCircle.SetActive(true);
            
            // Update size dynamically (in case range changes)
            float diameter = _stats.AttackRange.Value * 2f;
            _rangeCircle.transform.localScale = new Vector3(diameter, 0.05f, diameter);
            
            // Cyan Color (Debug style)
            // Ideally use a material, but this works for prototyping
             var renderer = _rangeCircle.GetComponent<Renderer>();
             renderer.material.color = new Color(0, 1, 1, 0.3f); // Cyan with transparency
        }
        else
        {
            if (_rangeCircle.activeSelf) _rangeCircle.SetActive(false);
        }
    }
}