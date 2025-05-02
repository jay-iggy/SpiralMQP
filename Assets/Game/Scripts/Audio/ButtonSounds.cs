using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerEnterHandler, IPointerClickHandler{
    private Sound selectSound;
    private Sound confirmSound;
    
    private void Start() {
        selectSound=Resources.Load<Sound>("SFX_UI_Select");
        confirmSound=Resources.Load<Sound>("SFX_UI_Confirm");
    }

    public void OnSelect(BaseEventData eventData) {
        // Play the button sound when hovered over
        Debug.Log("button_hover");
        selectSound.PlaySound();
    }

    public void OnSubmit(BaseEventData eventData) {
        // Play the button sound when clicked
        Debug.Log("button_click");
        confirmSound.PlaySound();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // Play the button sound when hovered over
        Debug.Log("button_hover");
        selectSound.PlaySound();
    }

    public void OnPointerClick(PointerEventData eventData) {
        // Play the button sound when clicked
        Debug.Log("button_click");
        confirmSound.PlaySound();
    }
}
