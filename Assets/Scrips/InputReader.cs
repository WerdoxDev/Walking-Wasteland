using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, InputActions.IPlayerActions, InputActions.IUIActions {
    public event Action<Vector2> MoveEvent;
    public event Action<ButtonType, bool> ButtonEvent;
    public event Action<int> OnWeaponChange;

    public event Action<Vector2> UIMoveEvent;
    public event Action<int> UIChangeTabEvent;
    public event Action<UIButtonType, bool> UIButtonEvent;

    private InputActions _controls;

    public void Init() {
        _controls = new InputActions();
        _controls.Player.SetCallbacks(this);
        _controls.UI.SetCallbacks(this);

        SetPlayerControlsActive(true);
        SetUIControlsActive(true);
    }

    public void OnMove(InputAction.CallbackContext context) => MoveEvent?.Invoke(context.ReadValue<Vector2>());

    public void OnShoot(InputAction.CallbackContext context) => SendButtonEvent(context, ButtonType.Shoot);
    public void OnReload(InputAction.CallbackContext context) => SendButtonEvent(context, ButtonType.Reload);

    public void SendButtonEvent(InputAction.CallbackContext context, ButtonType type) {
        if (context.performed) ButtonEvent?.Invoke(type, true);
        else if (context.canceled) ButtonEvent?.Invoke(type, false);
    }

    public void SendUIButtonEvent(InputAction.CallbackContext context, UIButtonType type) {
        if (context.performed) UIButtonEvent?.Invoke(type, true);
        else if (context.canceled) UIButtonEvent?.Invoke(type, false);
    }

    public void SendUIButtonEvent(UIButtonType type) => UIButtonEvent?.Invoke(type, true);

    public void SetPlayerControlsActive(bool isActive) {
        if (isActive) _controls.Player.Enable();
        else _controls.Player.Disable();
    }

    public void SetUIControlsActive(bool isActive) {
        if (isActive) _controls.UI.Enable();
        else _controls.UI.Disable();
    }

    public void OnNavigate(InputAction.CallbackContext context) => UIMoveEvent?.Invoke(context.ReadValue<Vector2>());
    public void OnSubmit(InputAction.CallbackContext context) => SendUIButtonEvent(context, UIButtonType.Submit);
    public void OnCancel(InputAction.CallbackContext context) => SendUIButtonEvent(context, UIButtonType.Cancel);
    public void OnApply(InputAction.CallbackContext context) => SendUIButtonEvent(context, UIButtonType.Apply);
    public void OnChangeTab(InputAction.CallbackContext context) => UIChangeTabEvent?.Invoke((int)context.ReadValue<float>());

    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }

    public void OnChangeWeapon(InputAction.CallbackContext context) {
        OnWeaponChange?.Invoke((int)context.ReadValue<Vector2>().normalized.y);
    }
}

public enum ButtonType {
    Chat,
    Pause,
    Shoot,
    Reload
}

public enum UIButtonType {
    Submit,
    Cancel,
    Apply,
}