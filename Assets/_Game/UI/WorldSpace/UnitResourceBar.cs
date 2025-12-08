using UnityEngine;
using UnityEngine.UI;

public class UnitResourceBar : MonoBehaviour
{
    [Header("Setup")]
    public UnitStats targetUnit;
    public Image fillImage;
    public Canvas canvas; // For billboarding

    [Header("Visual Config")]
    public Color manaColor = Color.blue;
    public Color energyColor = Color.yellow;
    public Color rageColor = Color.red;
    public Color cursedEnergyColor = Color.cyan;
    public Color furyColor = new Color(1f, 0.5f, 0f); // Orange
    public Color defaultColor = Color.white;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        if (targetUnit == null) targetUnit = GetComponentInParent<UnitStats>();
        
        if (targetUnit != null)
        {
            SetupVisuals();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void SetupVisuals()
    {
        if (fillImage == null) return;

        // Auto-set color based on Definition
        switch (targetUnit.definition.resourceType)
        {
            case ResourceType.Mana: fillImage.color = manaColor; break;
            case ResourceType.Energy: fillImage.color = energyColor; break;
            case ResourceType.Rage: fillImage.color = rageColor; break;
            case ResourceType.Fury: fillImage.color = furyColor; break;
            case ResourceType.CursedEnergy: fillImage.color = cursedEnergyColor; break;
            case ResourceType.None: 
                gameObject.SetActive(false); // Hide bar if unit has no resource
                break;
            default: fillImage.color = defaultColor; break;
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;

        if (targetUnit == null) return;

        // Update Fill
        if (fillImage != null)
        {
            float max = targetUnit.MaxResource.Value;
            // Prevent divide by zero
            if (max > 0)
            {
                float pct = targetUnit.CurrentResource / max;
                fillImage.fillAmount = pct;
            }
            else
            {
                fillImage.fillAmount = 0;
            }
        }
    }
}