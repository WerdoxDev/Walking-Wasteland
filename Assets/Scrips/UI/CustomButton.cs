using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    [SerializeField] private Graphic graphic;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color enterColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Vector3 enterScale;
    [SerializeField] private LeanTweenType easeType;
    [SerializeField] private float duration;
    [SerializeField] private bool changeColor = true;
    [SerializeField] private bool changeScale = true;
    [SerializeField] private bool tween = true;
    [SerializeField] private bool resetOnDisable = true;

    [Tooltip("If an advanced button exists on the parent, graphic locks it self in parent to avoid bugs")]
    [SerializeField] private bool lockIfParentExists = true;

    private bool _hasTweener = true;
    private UIOrder _order;
    private AdvancedCustomButton _parentButton;
    private UITweener _tweener;

    public bool IsSelected;
    public bool IsDisabled;
    public bool ResetOnDisable => resetOnDisable;
    public Graphic Graphic => graphic;

    public event Action OnClick;

    private void Awake() {
        _order = GetComponent<UIOrder>();
        _parentButton = GetComponentInParent<AdvancedCustomButton>();
    }

    private void OnDisable() {
        if (resetOnDisable) {
            IsSelected = false;
            Exit();
        };
    }

    public void Submit() {
        if (IsDisabled) return;

        OnClick?.Invoke();
    }

    public void Cancel() {
        if (IsDisabled) return;

        IsSelected = false;
        Exit();
    }

    public void SetDisabled(bool isDisabled) {
        IsDisabled = isDisabled;
        if (isDisabled) graphic.color = disabledColor;
        else graphic.color = normalColor;
    }

    public void Enter(bool setSelected = false) {
        if (IsDisabled) return;

        if (_tweener == null && _hasTweener) _hasTweener = TryGetComponent(out _tweener);

        if (setSelected) IsSelected = true;

        if (graphic != null) {
            if (changeColor) LeanTween.cancel(graphic.gameObject, false, TweenAction.COLOR);
            if (changeScale) LeanTween.cancel(graphic.gameObject, false, TweenAction.SCALE);
        }

        if (_tweener != null) _tweener.Lock();
        if (_parentButton != null && lockIfParentExists) _parentButton.LockGraphic(graphic.gameObject);

        if (!tween) {
            if (changeColor) graphic.color = enterColor;
            if (changeScale) graphic.rectTransform.localScale = enterScale;
            return;
        }

        if (changeColor) LeanTween.graphicColor(graphic.rectTransform, enterColor, duration).setEase(easeType).setRecursive(false);

        if (changeScale) {
            if (_order != null) _order.Order();
            LeanTween.scale(graphic.rectTransform, enterScale, duration).setEase(easeType);
        }
    }

    public void Exit() {
        if (IsSelected || IsDisabled) return;

        if (_tweener != null) _tweener.Unlock();
        if (_parentButton != null && lockIfParentExists) _parentButton.UnlockGraphic(graphic.gameObject);

        if (!tween) {
            if (changeColor) graphic.color = normalColor;
            if (changeScale) graphic.rectTransform.localScale = Vector3.one;
            return;
        };

        if (changeColor) LeanTween.graphicColor(graphic.rectTransform, normalColor, duration).setEase(easeType).setRecursive(false);

        if (changeScale) LeanTween.scale(graphic.rectTransform, Vector3.one, duration).setEase(easeType).setOnComplete(() => {
            if (_order != null) _order.RemoveOrder();
        });
    }

    public void OnPointerClick(PointerEventData eventData) => Submit();

    public void OnPointerEnter(PointerEventData eventData) => Enter();

    public void OnPointerExit(PointerEventData eventData) => Exit();

    // Selectable parent will prevent OnPointerClick if this doesn't exists
    public void OnPointerDown(PointerEventData eventData) { }
}
