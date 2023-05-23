using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PausePanel : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private CustomButton settingsButton;
    [SerializeField] private CustomButton leaveButton;
    [SerializeField] private Panel menuPanel;
    [SerializeField] private Panel settingsPanel;

    public Panel Panel;

    private void Awake() {
        Panel.OnClosed += () => HideMenuPanel();
        Panel.OnOpened += () => ShowMenuPanel();

        Panel.OnClosedWithButton += () => GameManager.Instance.ResumeGame();

        settingsButton.OnClick += () => ShowSettingsPanel();
        //leaveButton.OnClick += () => TheAbyssNetworkManager.Instance.Disconnect();
    }

    public bool IsMenuVisible() => menuPanel.IsOpen;

    public void HideMenuPanel() {
        menuPanel.Close();
        settingsPanel.Close();
    }

    public void ShowMenuPanel() {
        settingsPanel.Close();
        menuPanel.Open();
    }

    public void ShowSettingsPanel() {
        menuPanel.Close();
        settingsPanel.Open();
    }
}
