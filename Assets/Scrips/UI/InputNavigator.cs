using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class InputNavigator : MonoBehaviour {
    public static List<InputNavigator> Inputs = new();
    private TMP_InputField _inputField;

    public bool IsSelected;

    private void Awake() {
        _inputField = GetComponent<TMP_InputField>();

        Inputs.Add(this);
    }

    private void FixedUpdate() {
        IsSelected = EventSystem.current.currentSelectedGameObject == gameObject;
        if (EventSystem.current.currentSelectedGameObject != gameObject) return;
        if (Keyboard.current.tabKey.wasPressedThisFrame) {
            Selectable next = _inputField.FindSelectableOnDown();
            if (next != null) EventSystem.current.SetSelectedGameObject(next.gameObject);
        }
    }

    public static bool IsAnyInputSelected() => Inputs.Any(x => x.IsSelected);
}
