using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    [Header("UI")]
    //public StartPanel StartPanel;
    public Panel JoinPanel;
    public Panel HostPanel;
    public PausePanel PausePanel;
    public LobbyPanel LobbyPanel;
    public ChangeUsernamePanel ChangeUsernamePanel;
    public ChangeGameModePanel ChangeGameModePanel;
    //public LoadingPanel LoadingPanel;
    //public StatsPanel StatsPanel;
    //public ProfileCustomizePanel CustomizePanel;
    //public InteractionPanel InteractionPanel;
    public HUDPanel HUDPanel;
    public MainPanel MainPanel;
    //public GameObject InfoPanel;
    //public GameObject ChatPanel;
    public GameObject KeybindHelperPanelGO;

    [Header("Settings")]
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject playerNamePrefab;
    [SerializeField] private GameObject[] disableOnStart;

    public InputReader InputReader;
    public Camera Camera;
    public string Username;

    public event Action<TabGroupIndex, Tab, Action> OnChangeTabAttempt;
    public event Action<TabGroupIndex, Tab> OnTabChanged;
    public event Action<Panel, bool, Action> OnPanelChangeStateAttempt;

    private List<TabGroup> _tabGroups;
    private CustomButton _enteredCustomButton;
    private AdvancedCustomButton _enteredAdvCustomButton;
    private readonly List<Panel> _openedPanels = new();
    //private readonly Dictionary<string, PlayerNamePanel> _playerNames = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        eventSystem.SetActive(true);

        SetInputEvents(true);

        GameManager.Instance.OnPause += () => {
            HUDPanel.Hide();
            PausePanel.Panel.Open();
            KeybindHelperPanelGO.SetActive(true);
            GameManager.Instance.FreeCursor();
        };

        GameManager.Instance.OnResume += () => {
            HUDPanel.Show();
            PausePanel.Panel.Close();
            KeybindHelperPanelGO.SetActive(false);
            GameManager.Instance.LockCursor();
        };

        OnMainMenuState();

        KeybindHelperPanel.Instance.OnBackButtonClicked += () => InputReader.SendUIButtonEvent(UIButtonType.Cancel);

        GameManager.Instance.OnGameStateChanged += (state) => {
            if (state == GameState.InGame) OnInGameState();
            if (state == GameState.InMainMenu) OnMainMenuState();
            if (state == GameState.InLobby) OnLobbyState();
        };
    }

    private void OnMainMenuState() {
        MainPanel.gameObject.SetActive(true);
        LobbyPanel.gameObject.SetActive(false);
        HUDPanel?.Hide();
        KeybindHelperPanelGO?.SetActive(true);
        JoinPanel.Close(true);
        HostPanel.Close(true);
        PausePanel?.Panel.Close(true);
        //ChatPanel.SetActive(false);
        //LoadingPanel.Panel.Close(true);

        //InteractionPanel.ClearTarget();

        GameManager.Instance.IsPaused = false;
        GameManager.Instance.FreeCursor();
    }

    private void OnLobbyState() {
        LobbyPanel.gameObject.SetActive(true);
        MainPanel.gameObject.SetActive(false);
        JoinPanel.Close(true);
        HostPanel.Close(true);
        //LoadingPanel.Panel.Close(true);
    }

    private void OnInGameState() {
        HUDPanel?.Show();
        MainPanel.gameObject.SetActive(false);
        LobbyPanel.gameObject.SetActive(false);
        KeybindHelperPanelGO?.SetActive(false);
        JoinPanel.Close(true);
        HostPanel.Close(true);
        PausePanel?.Panel.Close(true);

        //ChatManager.Instance.ClearChat();
        //CrosshairManager.Instance.SetState(CrosshairState.Neutral);
    }

    public void AddTabGroup(TabGroup tabGroup) {
        if (_tabGroups == null) _tabGroups = new List<TabGroup>();
        if (_tabGroups.Exists(x => x.GroupIndex == tabGroup.GroupIndex)) return;
        _tabGroups.Add(tabGroup);
    }

    //public void AddPlayerName(Transform player, string name, Vector3 offset, bool ignoreEqual = false, Camera customCamera = null, RectTransform customRect = null) {
    //    if (name == PlayerName && !ignoreEqual) return;
    //    if (_playerNames.ContainsKey(name)) return;
    //    GameObject playerNameGO = Instantiate(playerNamePrefab, Vector3.zero, Quaternion.identity, transform);
    //    PlayerNamePanel playerNamePanel = playerNameGO.GetComponent<PlayerNamePanel>();
    //    playerNamePanel.SetPlayer(player, name, offset, customCamera, customRect);
    //    _playerNames.Add(name, playerNamePanel);
    //}

    //public void RemovePlayerName(string name) {
    //    PlayerNamePanel playerNamePanel = _playerNames.FirstOrDefault(x => x.Key == name).Value;
    //    if (playerNamePanel == null) return;
    //    _playerNames.Remove(name);
    //    Destroy(playerNamePanel.gameObject);
    //}

    public void ChangeTab(TabGroupIndex groupIndex, string tabName) => GetTabGroupByIndex(groupIndex)?.SelectTabByName(tabName);

    public void ChangeTabAttempt(TabGroupIndex groupIndex, Tab tab, Action cancel) => OnChangeTabAttempt?.Invoke(groupIndex, tab, cancel);
    public void PanelChangeStateAttempt(Panel panel, bool isOpen, Action cancel) => OnPanelChangeStateAttempt?.Invoke(panel, isOpen, cancel);

    public void TabChanged(TabGroupIndex groupIndex, Tab tab) => OnTabChanged?.Invoke(groupIndex, tab);

    public void AddOpenedPanel(Panel panel) => _openedPanels.Add(panel);
    public void RemoveOpenedPanel(Panel panel) => _openedPanels.Remove(panel);
    public void SetEnteredCustomButton(CustomButton customButton) => _enteredCustomButton = customButton;
    public void SetEnteredAdvCustomButton(AdvancedCustomButton advCustomButton) => _enteredAdvCustomButton = advCustomButton;

    //public Vector3 WorldToScreen(Vector3 worldPosition, Camera camera = null, RectTransform rect = null) {
    //    if (camera == null) camera = SettingsManager.Instance.CurrentCamera;
    //    if (camera == null) return Vector3.zero;

    //    if (rect == null) rect = RenderImage.rectTransform;

    //    Vector2 viewPos = camera.WorldToViewportPoint(worldPosition);
    //    Vector2 localPos = new(viewPos.x * rect.sizeDelta.x, viewPos.y * rect.sizeDelta.y);
    //    Vector3 worldPos = rect.TransformPoint(localPos);
    //    float scalerRatio = (1 / this.transform.lossyScale.x) * 2;

    //    return new Vector3
    //        (worldPos.x - rect.sizeDelta.x / scalerRatio,
    //         worldPos.y - rect.sizeDelta.y / scalerRatio, 1f);
    //}

    public bool IsInCameraBounds(Vector2 position, Camera camera = null) {
        if (camera == null) camera = Camera;
        if (camera == null) return false;

        return Vector2.Dot(camera.transform.forward, new Vector3(position.x, position.y) - camera.transform.position) >= 0;
    }

    private TabGroup GetTabGroupByIndex(TabGroupIndex groupIndex) => _tabGroups?.FirstOrDefault(x => x.GroupIndex == groupIndex);

    private void SetInputEvents(bool enabled) {
        bool buttonPauseInvoked = false;

        void OnButtonEvent(ButtonType type, bool performed) {
            if (GameManager.Instance.IsPaused || GameManager.Instance.GameState == GameState.InMainMenu) return;

            if (!performed) return;
            if (type == ButtonType.Pause) {
                if (ChatManager.Instance.IsOpen) {
                    ChatManager.Instance.Close();
                    GameManager.Instance.LocalPlayer.SetKeybindsActive(true);
                    return;
                }

                buttonPauseInvoked = true;
                if (!GameManager.Instance.IsPlayerSpawned || GameManager.Instance.IsPaused || !PausePanel.Panel.FullyClosed) return;
                GameManager.Instance.PauseGame();
            }
            else
            if (type == ButtonType.Chat) {
                if (GameManager.Instance.LocalPlayer == null) return;
                if (_openedPanels.Count != 0) return;

                if (!ChatManager.Instance.IsOpen) {
                    ChatManager.Instance.Open(true, false);
                    GameManager.Instance.FreeCursor();
                }
                else {
                    ChatManager.Instance.TrySendMessage();
                }
            }
        }

        void OnUIButtonEvent(UIButtonType type, bool performed) {
            if (!GameManager.Instance.IsPaused && GameManager.Instance.GameState != GameState.InMainMenu) return;

            if (!performed) return;
            if (type == UIButtonType.Cancel) {
                if (buttonPauseInvoked) {
                    buttonPauseInvoked = false;
                    return;
                }

                Panel openPanelOnTop = _openedPanels.Count == 0 ? null : _openedPanels[^1];
                if (openPanelOnTop != null && openPanelOnTop.CloseOnCancel) {
                    if (openPanelOnTop == PausePanel?.Panel) {
                        if (!PausePanel.IsMenuVisible()) PausePanel.ShowMenuPanel();
                        else if (PausePanel.Panel.FullyOpened) GameManager.Instance.ResumeGame();
                        return;
                    }

                    openPanelOnTop.Close();
                    return;
                }

                if (_enteredCustomButton != null) {
                    _enteredCustomButton.Cancel();
                    _enteredCustomButton = null;
                    return;
                }

                if (_enteredAdvCustomButton != null) {
                    _enteredAdvCustomButton.Cancel();
                    _enteredAdvCustomButton = null;
                    return;
                }


                if (MainPanel.ActivePanel != ActivePanel.MenuPanel) MainPanel.ShowMenuPanel();
                //if (GetTabGroupByIndex(TabGroupIndex.Menu).ActiveTab.name == "Home") { return; } // Prompt a exit confirm
                //ChangeTab(TabGroupIndex.Menu, "Home");
            }
            if (type == UIButtonType.Submit) {
                Tab enteredTab = _tabGroups?.FirstOrDefault(x => x.EnteredTab != null)?.EnteredTab;
                if (enteredTab != null) {
                    enteredTab.Submit();
                    return;
                }

                if (_enteredCustomButton != null) {
                    _enteredCustomButton.Submit();
                    return;
                }

                if (_enteredAdvCustomButton != null) {
                    _enteredAdvCustomButton.Submit();
                    return;
                }

                Panel openPanelOnTop = _openedPanels.Count == 0 ? null : _openedPanels[^1];
                if (openPanelOnTop != null) {
                    openPanelOnTop.Submit();
                    return;
                }
            }
        }

        void OnChangeTab(int direction) {
            if (InputNavigator.IsAnyInputSelected()) return;
            if (!GameManager.Instance.IsPaused && GameManager.Instance.GameState == GameState.InGame) return;
            if (_openedPanels.Count != 0) return;

            ////TabGroupIndex groupIndex = TabGroupIndex.Menu;
            //if (JoinPanel.IsOpen) groupIndex = TabGroupIndex.JoinPanel;
            //else if (HostPanel.IsOpen) groupIndex = TabGroupIndex.HostPanel;
            //if (direction == 1) GetTabGroupByIndex(groupIndex).SelectNextTab();
            //else if (direction == -1) GetTabGroupByIndex(groupIndex).SelectPreviousTab();
        }

        if (enabled) {
            InputReader.ButtonEvent += OnButtonEvent;
            InputReader.UIButtonEvent += OnUIButtonEvent;
            InputReader.UIChangeTabEvent += OnChangeTab;
        }
        else {
            InputReader.ButtonEvent -= OnButtonEvent;
            InputReader.UIButtonEvent -= OnUIButtonEvent;
            InputReader.UIChangeTabEvent -= OnChangeTab;
        }
    }
}

public enum TabGroupIndex {
    SettingsPanel
}
