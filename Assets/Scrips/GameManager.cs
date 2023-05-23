using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using ParrelSync;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header("Settings")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerSprite[] playerSprites;
    [SerializeField] private string tempUrl;
    [SerializeField] private bool autoSpawn;

    public event Action<Player, bool> OnPlayerSpawned;
    public event Action<Player, bool> OnPlayerDespawned;
    public event Action<string> OnSceneChanged;
    public event Action<GameState> OnGameStateChanged;
    public event Action OnPause;
    public event Action OnResume;

    public GameState GameState;
    public ulong LocalPlayerId;
    public Player LocalPlayer;
    public bool IsPlayerSpawned;
    public bool IsPaused;
    public PlayerSprite[] PlayerSprites => playerSprites;

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        inputReader.Init();

        WWNetworkManager.Instance.OnLocalClientConnected += (clientId) => SetGameState(GameState.InLobby);
        WWNetworkManager.Instance.OnLocalClientDisconnected += (clientId) => SetGameState(GameState.InMainMenu);
        LobbyManager.Instance.OnLobbyStarted += () => {
            SetGameState(GameState.InGame);

            if (LocalPlayerId == LobbyManager.Instance.HostLobbyPlayerId)
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        };

        OnPlayerSpawned += (player, isOwner) => {
            if (isOwner) LocalPlayer = player;
        };
    }

#if UNITY_EDITOR
    // Temporary
    private void Start() {
        if (!autoSpawn) return;

        string[] split = tempUrl.Split(":");
        SetConnectionData(split[0], ushort.Parse(split[1]));

        LobbyManager.Instance.OnLobbyPlayerInitialized += (clientId) => {
            if (clientId != NetworkManager.Singleton.LocalClientId) LobbyManager.Instance.StartLobby();
        };

        if (ClonesManager.IsClone()) WWNetworkManager.Instance.Client();
        else {
            WWNetworkManager.Instance.Host();
            LobbyManager.Instance.SetGameModeServerRpc(GameMode.PVE);
        }
    }
#endif

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FreeCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayerSpawned(Player player, bool isOwner) {
        OnPlayerSpawned?.Invoke(player, isOwner);
    }

    public void PlayerDespawned(Player player) {
        OnPlayerDespawned?.Invoke(player, player.OwnerClientId == LocalPlayerId);
    }

    public void PauseGame() {
        IsPaused = true;
        OnPause?.Invoke();
    }

    public void ResumeGame() {
        IsPaused = false;
        OnResume?.Invoke();
    }

    public void SceneChanged(string sceneName) => OnSceneChanged?.Invoke(sceneName);

    public void SetGameState(GameState newState) {
        GameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void SetConnectionData(string address, ushort port) {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = address;
        transport.ConnectionData.Port = port;
    }

    public PlayerSprite GetPlayerSpriteByType(PlayerSpriteType type) {
        return playerSprites.FirstOrDefault(x => x.Type == type);
    }

    public int GetPlayerSpriteIndexByType(PlayerSpriteType type) {
        return playerSprites.ToList().FindIndex(x => x.Type == type);
    }
}

public enum GameState {
    InMainMenu,
    InGame,
    InLobby
}