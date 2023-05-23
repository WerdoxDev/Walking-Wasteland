using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour {
    [SerializeField] private TabGroupIndex groupIndex;
    [SerializeField] private GameObject[] panels;
    private int _currentIndex;

    private void Awake() {
        UIManager.Instance.OnTabChanged += (groupIndex, tab) => {
            if (this.groupIndex == groupIndex) SetPageIndex(tab.transform.GetSiblingIndex());
        };

        ShowCurrentPanel();
    }

    public void SetPageIndex(int index) {
        _currentIndex = index;
        ShowCurrentPanel();
    }

    private void ShowCurrentPanel() {
        for (int i = 0; i < panels.Length; i++) {
            if (i == _currentIndex) panels[i].gameObject.SetActive(true);
            else panels[i].gameObject.SetActive(false);
        }
    }
}