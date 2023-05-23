using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownItem : MonoBehaviour {
    [Header("General")]
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private GameObject checkmark;
    public CustomButton Button { get; private set; }

    public string Value { get; private set; }
    public bool IsSelected { get; private set; }

    public void Initialize(string value, bool isDefault) {
        Value = value;
        valueText.text = value;
        Button = GetComponent<CustomButton>();

        if (isDefault) Select();
        else Deselect();
    }

    public void Select() {
        checkmark.SetActive(true);
        IsSelected = true;
    }

    public void Deselect() {
        checkmark.SetActive(false);
        IsSelected = false;
    }
}
