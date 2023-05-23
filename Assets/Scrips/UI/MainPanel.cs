using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour {

    [Header("General")]
    [SerializeField] private CustomButton playButton;
    [SerializeField] private CustomButton quitButton;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject playPanel;
    public ActivePanel ActivePanel;

    private void Awake() {
        playButton.OnClick += () => ShowPlayPanel();
        quitButton.OnClick += () => Application.Quit();
        ShowMenuPanel();
    }

    public void ShowMenuPanel() {
        menuPanel.SetActive(true);
        playPanel.SetActive(false);
        ActivePanel = ActivePanel.MenuPanel;
    }

    public void ShowPlayPanel() {
        menuPanel.SetActive(false);
        playPanel.SetActive(true);
        ActivePanel = ActivePanel.PlayPanel;
    }
}

public enum ActivePanel {
    MenuPanel,
    PlayPanel,
    SettingsPanel,
}