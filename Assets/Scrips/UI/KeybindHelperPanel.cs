using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class KeybindHelperPanel : MonoBehaviour {
    public static KeybindHelperPanel Instance;

    [SerializeField] private CustomButton backButton;
    [SerializeField] private CustomButton rtdButton;
    [SerializeField] private CustomButton applyButton;

    public bool IsApplyVisible;
    public bool IsRTDVisible;
    public bool IsBackVisible;

    public event Action OnApplyButtonClicked;
    public event Action OnBackButtonClicked;
    public event Action OnRTDButtonClicked;

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        applyButton.OnClick += () => OnApplyButtonClicked?.Invoke();
        backButton.OnClick += () => OnBackButtonClicked?.Invoke();
        rtdButton.OnClick += () => OnRTDButtonClicked?.Invoke();

        SetApplyButtonActive(false);
        SetRTDButtonActive(false);
    }

    public void SetApplyButtonActive(bool isActive) {
        applyButton.gameObject.SetActive(isActive);
        IsApplyVisible = isActive;
    }

    public void SetBackButtonActive(bool isActive) {
        backButton.gameObject.SetActive(isActive);
        IsBackVisible = isActive;
    }
    public void SetRTDButtonActive(bool isActive) {
        rtdButton.gameObject.SetActive(isActive);
        IsRTDVisible = isActive;
    }
}
