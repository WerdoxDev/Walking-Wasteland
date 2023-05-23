using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour {
    public NetworkVariable<GameMode> CurrentGameMode = new(GameMode.Deathmatch);

    public static LobbyManager Instance;

    public ulong HostLobbyPlayerId;

    public event Action OnLobbyStarted;
    public event Action<ulong> OnLobbyPlayerInitialized;

    public List<LobbyPlayer> LobbyPlayers = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn() {
        CurrentGameMode.OnValueChanged += (GameMode oldMode, GameMode newMode) => {
            UIManager.Instance.LobbyPanel.UpdateGameMode();
        };
    }

    public void AddPlayer(LobbyPlayer lobbyPlayer) {
        LobbyPlayers.Add(lobbyPlayer);
        if (lobbyPlayer.Data.Value.IsHost) HostLobbyPlayerId = lobbyPlayer.OwnerClientId;

        OnLobbyPlayerInitialized?.Invoke(lobbyPlayer.OwnerClientId);
    }

    public void RemovePlayer(LobbyPlayer lobbyPlayer) {
        LobbyPlayers.Remove(lobbyPlayer);
    }

    public void StartLobby() => OnLobbyStarted?.Invoke();

    public bool IsEveryoneReady() => LobbyPlayers.All(x => x.Data.Value.IsReady);

    public GameMode GetGameModeByName(string name) {
        return
            name.ToLower() == "deathmatch" ? GameMode.Deathmatch :
            name.ToLower() == "pvp" ? GameMode.PVP :
            name.ToLower() == "pve" ? GameMode.PVE :
            GameMode.None;
    }

    public LobbyPlayer GetLobbyPlayer(ulong id) {
        return LobbyPlayers.FirstOrDefault(x => x.OwnerClientId == id);
    }

    [ClientRpc]
    public void StartCountdownClientRpc() {
        UIManager.Instance.LobbyPanel.StartCountdown();
    }

    [ClientRpc]
    public void StopCountdownClientRpc() {
        UIManager.Instance.LobbyPanel.StopCountdown();
    }

    [ServerRpc]
    public void SetGameModeServerRpc(GameMode gameMode) {
        CurrentGameMode.Value = gameMode;
    }
}

public enum GameMode {
    None,
    Deathmatch,
    PVP,
    PVE
}
