using UnityEngine;
using UnityEngine.InputSystem;

public class UnitAbilityManager : MonoBehaviour
{
    [Header("Loadout")]
    public AbilityDefinition abilityQ;
    public AbilityDefinition abilityW;
    public AbilityDefinition abilityE;
    public AbilityDefinition abilityR;

    [Header("Runtime State")]
    public float cooldownQ;
    public float cooldownW;
    public float cooldownE;
    public float cooldownR;

    private UnitStats _stats;
    private AbilityDefinition _pendingAbility; // The spell we are currently aiming
    private Camera _cam;
    private InputHandler _inputHandler; // To get mouse pos

    // A simple visual aid for debugging range (Replace with fancy UI later)
    private GameObject _rangeIndicator; 

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        _cam = Camera.main;
        _inputHandler = FindObjectOfType<InputHandler>(); // We need the mouse logic
        
        // Create a temporary debug circle for aiming
        _rangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _rangeIndicator.name = "RangeIndicator";
        
        // CHANGE 0.1f TO 0.01f HERE:
        _rangeIndicator.transform.localScale = new Vector3(1, 0.01f, 1); 
        
        Destroy(_rangeIndicator.GetComponent<Collider>()); 
        _rangeIndicator.SetActive(false);
    }

    void Start()
    {
        // Initialize all abilities in the loadout
        if (abilityQ != null) abilityQ.OnEquip(_stats);
        if (abilityW != null) abilityW.OnEquip(_stats);
        if (abilityE != null) abilityE.OnEquip(_stats);
        if (abilityR != null) abilityR.OnEquip(_stats);
    }

    void Update()
    {
        HandleCooldowns();
        HandleInput();
        
        if (_pendingAbility != null)
        {
            UpdateAimingVisuals();
        }
    }

    private void HandleCooldowns()
    {
        if (cooldownQ > 0) cooldownQ -= Time.deltaTime;
        if (cooldownW > 0) cooldownW -= Time.deltaTime;
        if (cooldownE > 0) cooldownE -= Time.deltaTime;
        if (cooldownR > 0) cooldownR -= Time.deltaTime;
    }

    private void HandleInput()
    {
        // 1. Cancel Aiming (Right Click moves, so it should cancel aim)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _pendingAbility = null;
            _rangeIndicator.SetActive(false);
            return;
        }

        // 2. Confirm Cast (Left Click)
        if (Mouse.current.leftButton.wasPressedThisFrame && _pendingAbility != null)
        {
            ExecuteCast(_pendingAbility);
            return;
        }

        // 3. Select Spell to Aim (Q, W, E, R)
        if (Keyboard.current.qKey.wasPressedThisFrame) PrepareCast(abilityQ, cooldownQ);
        if (Keyboard.current.wKey.wasPressedThisFrame) PrepareCast(abilityW, cooldownW);
        if (Keyboard.current.eKey.wasPressedThisFrame) PrepareCast(abilityE, cooldownE);
        if (Keyboard.current.rKey.wasPressedThisFrame) PrepareCast(abilityR, cooldownR);
    }

    private void PrepareCast(AbilityDefinition ability, float currentCD)
    {
        if (ability == null) return;
        if (currentCD > 0) 
        {
            Debug.Log("Ability on Cooldown!");
            return;
        }
        
        // --- FIX: Check for Instant Cast ---
        if (ability.targetingMode == TargetingMode.NoTarget)
        {
            // Cast immediately without entering aiming mode
            ExecuteCast(ability); 
            return;
        }
        // -----------------------------------

        if (_stats.CurrentResource < ability.manaCost)
        {
            Debug.Log("Not enough Resource!");
            return;
        }

        // Enter "Aiming Mode"
        _pendingAbility = ability;
        
        _rangeIndicator.SetActive(true);
        float size = ability.castRange * 2; 
        _rangeIndicator.transform.localScale = new Vector3(size, 0.1f, size);
    }

    private void UpdateAimingVisuals()
    {
        // Keep the circle on the player, BUT LIFT IT UP slightly
        Vector3 newPos = transform.position;
        newPos.y += 0.1f; // Lift it up!
        _rangeIndicator.transform.position = newPos;
    }

    private void ExecuteCast(AbilityDefinition ability)
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        Vector3 point = Vector3.zero;
        UnitStats targetUnit = null;

        // 1. RaycastAll to find units (prioritize units over ground)
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        
        foreach (var hit in hits)
        {
            UnitStats unit = hit.collider.GetComponent<UnitStats>();
            if (unit != null && unit.gameObject != gameObject) 
            {
                targetUnit = unit;
                point = hit.point; 
                break; 
            }
        }

        // If no unit found, try to find ground
        if (targetUnit == null)
        {
            foreach (var hit in hits)
            {
                // Check if layer is Default (Layer 0) or Ground
                if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) 
                {
                    point = hit.point;
                    break;
                }
            }
        }

        // 2. Validate Targeting
        if (ability.targetingMode == TargetingMode.TargetUnit)
        {
            if (targetUnit == null)
            {
                Debug.Log("Needs a target!");
                return;
            }

            // NEW CODE: Check Friendly Fire
            if (!TeamLogic.IsEnemy(_stats.team, targetUnit.team))
            {
                Debug.Log("Cannot target ally!");
                return;
            }
        }
        
        // 3. Cast
        ability.OnCast(_stats, point, targetUnit);

        // 4. Set Cooldowns
        if (ability == abilityQ) cooldownQ = ability.cooldown;
        if (ability == abilityW) cooldownW = ability.cooldown;
        if (ability == abilityE) cooldownE = ability.cooldown;
        if (ability == abilityR) cooldownR = ability.cooldown;

        _pendingAbility = null;
        _rangeIndicator.SetActive(false);
    }
}