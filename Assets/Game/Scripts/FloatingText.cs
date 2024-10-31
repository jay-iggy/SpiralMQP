using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyDelay = 3f;

    private TMP_Text _textObj;

    private void Awake() {
        _textObj = GetComponent<TMP_Text>();
    }

    public void SetText(string text) {
        _textObj.text = text;
    }
    
    
    void Start() {
        Destroy(gameObject,destroyDelay);
    }
}
