using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeTime = 1f;
    
    private TextMeshProUGUI _text;
    private float _timer;
    private Color _startColor;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _startColor = _text.color;
    }

    public void Setup(float damageAmount)
    {
        _text.text = Mathf.RoundToInt(damageAmount).ToString();
        _timer = fadeTime;
    }

    void Update()
    {
        // Float Up
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Fade Out
        _timer -= Time.deltaTime;
        float alpha = _timer / fadeTime;
        _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);

        if (_timer <= 0) Destroy(gameObject);
    }
}