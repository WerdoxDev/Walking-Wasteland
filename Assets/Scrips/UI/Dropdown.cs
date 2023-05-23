using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Dropdown : MonoBehaviour {
    [Header("General")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemsWrapper;
    [SerializeField] private Transform content;
    [SerializeField] private TMP_Text selectedValueText;
    [SerializeField] private UITweener openTweener;
    [SerializeField] private UITweener closeTweener;
    [SerializeField] private DropdownValue[] values;

    private readonly List<DropdownItem> dropdownItems = new();
    private CustomButton button;
    private bool isOpen;

    public string SelectedValue { get; private set; }

    private void Awake() {
        button = GetComponent<CustomButton>();
        Close();

        button.OnClick += () => {
            if (isOpen) Close();
            else Open();
        };

        foreach (DropdownValue value in values) {
            DropdownItem dropdownItem = Instantiate(itemPrefab, content).GetComponent<DropdownItem>();
            dropdownItem.Initialize(value.Value, value.IsDefault);
            dropdownItem.Button.OnClick += () => SelectItem(dropdownItem);
            dropdownItems.Add(dropdownItem);
        }

        SelectItem(dropdownItems[0]);
    }

    private void Open() {
        if (isOpen) return;

        openTweener.HandleTween();
        isOpen = true;
    }

    private void Close() {
        if (!isOpen) return;

        closeTweener.HandleTween();
        isOpen = false;
    }

    private void SelectItem(DropdownItem dropdownItem) {
        dropdownItem.Select();
        dropdownItems.ForEach(item => {
            if (item != dropdownItem) item.Deselect();
        });

        selectedValueText.text = dropdownItem.Value;
        SelectedValue = dropdownItem.Value;
        Close();
    }

    public void SelectItemByValue(string value) {
        DropdownItem dropdownItem = dropdownItems.FirstOrDefault(x => x.Value.ToLower() == value.ToLower());
        if (dropdownItem != null) SelectItem(dropdownItem);
    }
}

[Serializable]
public struct DropdownValue {
    public string Value;
    public bool IsDefault;
}
