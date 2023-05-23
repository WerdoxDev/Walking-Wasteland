using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CustomButton), typeof(Selectable))]
public class CustomButtonSelectable : MonoBehaviour, ISelectHandler, IDeselectHandler {

    public CustomButton CustomButton { get; private set; }

    private void Awake() {
        CustomButton = GetComponent<CustomButton>();
    }

    public void OnSelect(BaseEventData eventData) {
        if (CustomButton == null) CustomButton = GetComponent<CustomButton>();

        CustomButton.Enter(true);
        UIManager.Instance.SetEnteredCustomButton(CustomButton);
    }

    public void OnDeselect(BaseEventData eventData) {
        CustomButton.IsSelected = false;
        CustomButton.Exit();
        UIManager.Instance.SetEnteredCustomButton(null);
    }

    private void OnDisable() {
        if (CustomButton != null && CustomButton.ResetOnDisable) OnDeselect(null);
    }
}
