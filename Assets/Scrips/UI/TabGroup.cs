using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class TabGroup : MonoBehaviour {
    [Tooltip("Important for event system to know which tab group this is")]
    public TabGroupIndex GroupIndex;
    [SerializeField] private bool resetOnDisable;

    public Tab ActiveTab { get; private set; }
    public Tab EnteredTab { get; private set; }

    private List<Tab> _tabs;
    private int _activeTabIndex;
    private bool _cancelled;

    private void OnDisable() {
        if (resetOnDisable) OnTabSelected(_tabs.FirstOrDefault(x => x.DefaultTab));
    }

    private void Awake() {
        UIManager.Instance.AddTabGroup(this);
    }

    private void Start() {
        ResetTabs();
    }

    public void AddTab(Tab tab) {
        if (_tabs == null) _tabs = new List<Tab>();

        if (tab.DefaultTab) OnTabSelected(tab, true, false);
        _tabs.Add(tab);
    }

    public void OnTabEnter(Tab tab) {
        if (tab.name == ActiveTab?.name) {
            tab.SetState(TabState.ActiveHover);
            return;
        }

        EnteredTab = tab;
        ResetTabs();
        tab.SetState(TabState.Hover);
    }

    public void OnTabExit(Tab tab) {
        if (EnteredTab == tab) EnteredTab = null;
        ResetTabs();
    }

    public void OnTabSelected(Tab tab, bool setPanel = true, bool selectTab = true) {
        if (ActiveTab == tab) return;

        UIManager.Instance.ChangeTabAttempt(GroupIndex, tab, () => _cancelled = true);

        if (_cancelled) {
            _cancelled = false;
            return;
        }

        ActiveTab = tab;
        _activeTabIndex = GetTabIndex(tab);
        ResetTabs();
        if (selectTab) EventSystem.current?.SetSelectedGameObject(tab.gameObject);
        ActiveTab.SetState(TabState.ActiveHover);
        UIManager.Instance.TabChanged(GroupIndex, tab);
    }

    public void SelectNextTab() {
        _activeTabIndex++;
        if (_activeTabIndex == _tabs.Count) _activeTabIndex = 0;

        SelectTabByIndex(_activeTabIndex);
    }

    public void SelectPreviousTab() {
        _activeTabIndex--;
        if (_activeTabIndex < 0) _activeTabIndex = _tabs.Count - 1;

        SelectTabByIndex(_activeTabIndex);
    }

    public Tab SelectTabByName(string name) {
        Tab tab = _tabs.FirstOrDefault(x => x.name == name);
        if (tab == null) return null;

        OnTabSelected(tab, false);
        return tab;
    }

    public void SelectTabByIndex(int index) {
        Tab tab = _tabs.FirstOrDefault(x => x.name == transform.GetChild(index).name);
        if (tab == null) return;

        ActiveTab = tab;
        ResetTabs();
        EventSystem.current.SetSelectedGameObject(tab.gameObject);
        ActiveTab.SetState(TabState.ActiveHover);
        UIManager.Instance.TabChanged(GroupIndex, tab);
    }

    public void ResetTabs() {
        foreach (Tab tab in _tabs)
            if (tab.name != ActiveTab?.name)
                tab.SetState(TabState.Inactive);
            else
                tab.SetState(TabState.Active);
    }

    private int GetTabIndex(Tab tab) {
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).name == tab.name) return i;
        }

        return -1;
    }
}
