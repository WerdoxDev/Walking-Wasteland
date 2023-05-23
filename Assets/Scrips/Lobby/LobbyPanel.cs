using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyPanel : MonoBehaviour {
    [Header("General")]
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private Transform playersWrapper;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Color neutralColor;
    [SerializeField] private Color[] teamColors;

    [Header("Buttons")]
    [SerializeField] private CustomButton StartButton;
    [SerializeField] private CustomButton CancelButton;
    [SerializeField] private CustomButton ReadyButton;
    [SerializeField] private CustomButton UnreadyButton;
    [SerializeField] private CustomButton LeaveButton;
    [SerializeField] private CustomButton ChangeUsernameButton;
    [SerializeField] private CustomButton ChangeGameModeButton;

    private IEnumerator countdownCoroutine;

    private List<LobbyPlayerVisual> lobbyPlayerVisuals = new();

    public ulong LocalPlayerVisualId { get; private set; }

    public Color[] TeamColors => teamColors;
    public Color NeutralColor => neutralColor;

    private void Awake() {
        UnreadyButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);

        ReadyButton.OnClick += () => {
            GetPlayerVisual(LocalPlayerVisualId).Player.SetIsReady(true);
            ReadyButton.gameObject.SetActive(false);
            UnreadyButton.gameObject.SetActive(true);
        };

        UnreadyButton.OnClick += () => {
            GetPlayerVisual(LocalPlayerVisualId).Player.SetIsReady(false);
            ReadyButton.gameObject.SetActive(true);
            UnreadyButton.gameObject.SetActive(false);
        };

        LeaveButton.OnClick += () => WWNetworkManager.Instance.Disconnect();

        ChangeUsernameButton.OnClick += () => UIManager.Instance.ChangeUsernamePanel.Panel.Open();
        ChangeGameModeButton.OnClick += () => UIManager.Instance.ChangeGameModePanel.Panel.Open();

        StartButton.OnClick += () => LobbyManager.Instance.StartCountdownClientRpc();
        CancelButton.OnClick += () => LobbyManager.Instance.StopCountdownClientRpc();

        WWNetworkManager.Instance.OnClientDisconnected += (clientId) => {
            DeletePlayer(GetPlayerVisual(clientId));
        };

        WWNetworkManager.Instance.OnLocalClientDisconnected += (clientId) => {
            foreach (LobbyPlayerVisual lobbyPlayerVisual in lobbyPlayerVisuals.ToList()) DeletePlayer(lobbyPlayerVisual);
        };

        gameModeText.text = $"Game Mode: {LobbyManager.Instance.CurrentGameMode.Value}";
    }

    public void AddPlayer(LobbyPlayerData lobbyPlayerData, LobbyPlayer lobbyPlayer, bool hasAuthority) {
        LobbyPlayerVisual lobbyPlayerVisual = Instantiate(lobbyPlayerPrefab, playersWrapper).GetComponent<LobbyPlayerVisual>();
        GameMode gameMode = LobbyManager.Instance.CurrentGameMode.Value;

        lobbyPlayerVisual.SetTeamActive(gameMode == GameMode.PVP);
        lobbyPlayerVisual.SetTeamButtonsActive(hasAuthority);
        lobbyPlayerVisual.SetCharButtonsActive(hasAuthority);

        lobbyPlayerVisual.SetUsername(lobbyPlayerData.Username.ToString());
        lobbyPlayerVisual.SetTeam(lobbyPlayerData.Team);
        lobbyPlayerVisual.SetSprite(GameManager.Instance.GetPlayerSpriteByType(lobbyPlayerData.Sprite).Stand, lobbyPlayerData.Sprite);
        lobbyPlayerVisual.SetReady(lobbyPlayerData.IsReady);
        lobbyPlayerVisual.SetColorScheme(gameMode != GameMode.PVP ? neutralColor : teamColors[lobbyPlayerData.Team]);

        lobbyPlayerVisual.Player = lobbyPlayer;

        lobbyPlayerVisuals.Add(lobbyPlayerVisual);

        if (hasAuthority) {
            LocalPlayerVisualId = lobbyPlayer.OwnerClientId;
            GameManager.Instance.LocalPlayerId = lobbyPlayer.OwnerClientId;
        }

        if (LocalPlayerVisualId == lobbyPlayer.OwnerClientId && !lobbyPlayer.Data.Value.IsHost) {
            StartButton.gameObject.SetActive(false);
            ChangeGameModeButton.gameObject.SetActive(false);
        }

        EnableStartIfNeeded();

        LobbyManager.Instance.AddPlayer(lobbyPlayer);
    }

    public void UpdatePlayer(ulong id, LobbyPlayerData lobbyPlayerData) {
        LobbyPlayerVisual lobbyPlayerVisual = GetPlayerVisual(id);
        GameMode gameMode = LobbyManager.Instance.CurrentGameMode.Value;

        lobbyPlayerVisual.SetUsername(lobbyPlayerData.Username.ToString());
        lobbyPlayerVisual.SetTeam(lobbyPlayerData.Team);
        lobbyPlayerVisual.SetSprite(GameManager.Instance.GetPlayerSpriteByType(lobbyPlayerData.Sprite).Stand, lobbyPlayerData.Sprite);
        lobbyPlayerVisual.SetReady(lobbyPlayerData.IsReady);
        lobbyPlayerVisual.SetColorScheme(gameMode != GameMode.PVP ? neutralColor : teamColors[lobbyPlayerData.Team]);

        if (!lobbyPlayerData.IsReady) LobbyManager.Instance.StopCountdownClientRpc();

        lobbyPlayerVisual.SetTeamActive(gameMode == GameMode.PVP);

        EnableStartIfNeeded();
    }

    public void DeletePlayer(LobbyPlayerVisual lobbyPlayerVisual) {
        lobbyPlayerVisuals.Remove(lobbyPlayerVisual);
        Destroy(lobbyPlayerVisual.gameObject);
    }

    public void UpdateGameMode() {
        gameModeText.text = $"Game Mode: {LobbyManager.Instance.CurrentGameMode.Value}";

        foreach (LobbyPlayerVisual lobbyPlayerVisual in lobbyPlayerVisuals)
            UpdatePlayer(lobbyPlayerVisual.Player.OwnerClientId, lobbyPlayerVisual.Player.Data.Value);
    }

    public void StartCountdown() {
        countdownCoroutine = StartCountdownEnumerator(5);
        StartCoroutine(countdownCoroutine);
        countdownText.gameObject.SetActive(true);

        if (LocalPlayerVisualId != LobbyManager.Instance.HostLobbyPlayerId) return;
        StartButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(true);
    }

    public void StopCountdown() {
        if (countdownCoroutine == null) return;
        StopCoroutine(countdownCoroutine);
        countdownText.gameObject.SetActive(false);

        if (LocalPlayerVisualId != LobbyManager.Instance.HostLobbyPlayerId) return;
        StartButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(false);
    }

    public void EnableStartIfNeeded() {
        StartButton.SetDisabled(LobbyManager.Instance.HostLobbyPlayerId != LocalPlayerVisualId || !LobbyManager.Instance.IsEveryoneReady());
    }

    public LobbyPlayerVisual GetPlayerVisual(ulong id) {
        return lobbyPlayerVisuals.FirstOrDefault(x => x.Player.OwnerClientId == id);
    }

    public IEnumerator StartCountdownEnumerator(int timeInSeconds) {
        int countdown = timeInSeconds;

        countdownText.text = $"Starting in... {countdown}";
        while (countdown > 0) {
            yield return new WaitForSeconds(1);
            countdown--;
            countdownText.text = $"Starting in... {countdown}";
        }

        LobbyManager.Instance.StartLobby();
    }
}

