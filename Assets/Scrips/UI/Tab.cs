using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {
    public string Name;
    public bool DefaultTab;
    public TabGroup TabGroup;

    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color[] activeColors;
    [SerializeField] private Color[] inactiveColors;
    [SerializeField] private Color[] hoverColors;
    [SerializeField] private Color[] activeHoverColors;

    private void Awake() {
        TabGroup.AddTab(this);
    }

    public void SetState(TabState state) {
        switch (state) {
            case TabState.Hover:
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = hoverColors[i];
                break;
            case TabState.ActiveHover:
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = activeHoverColors[i];
                break;
            case TabState.Active:
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = activeColors[i];
                break;
            case TabState.Inactive:
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = inactiveColors[i];
                break;
        }
    }

    public void Submit() => TabGroup.OnTabSelected(this);

    public void OnPointerClick(PointerEventData eventData) => Submit();

    public void OnPointerEnter(PointerEventData eventData) => TabGroup.OnTabEnter(this);

    public void OnPointerExit(PointerEventData eventData) => TabGroup.OnTabExit(this);

    public void OnSelect(BaseEventData eventData) => TabGroup.OnTabEnter(this);

    public void OnDeselect(BaseEventData eventData) => TabGroup.OnTabExit(this);

    // public void OnCancel(BaseEventData eventData) => TabGroup.OnTabExit(this);

    // public void OnSubmit(BaseEventData eventData) => TabGroup.OnTabSelected(this);
}

public enum TabState {
    Hover, Active, Inactive, ActiveHover
}
