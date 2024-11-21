using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyDelay = 3f;

    private TextMeshPro _textObj;

    private void Awake() {
        _textObj = GetComponent<TextMeshPro>();
    }

    public void SetText(string text) {
        _textObj.text = text;
    }

    public void SetColor(Color color) {
        _textObj.color = color;
    }
    
    
    void Start() {
        Destroy(gameObject,destroyDelay);
    }
}
