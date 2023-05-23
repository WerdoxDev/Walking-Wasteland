using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WWNetworkManager : MonoBehaviour {
    public static WWNetworkManager Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject lobbyPlayerPrefab;

    public event Action<ulong> OnClientConnected;
    public event Action<ulong> OnLocalClientConnected;
    public event Action<ulong> OnClientDisconnected;
    public event Action<ulong> OnLocalClientDisconnected;
    public event Action OnClientConnectionStarted;
    public event Action OnHostConnectionStarted;

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        NetworkManager.Singleton.OnClientStopped += (wasHost) => OnLocalClientDisconnected?.Invoke(NetworkManager.Singleton.LocalClientId);
    }

    public void Host() {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += HandleSceneLoadComplete;

        OnHostConnectionStarted?.Invoke();
    }

    public void Client() {
        NetworkManager.Singleton.StartClient();
        OnClientConnectionStarted?.Invoke();
    }

    public void Disconnect() {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= HandleSceneLoadComplete;
    }

    private void HandleClientConnected(ulong clientId) {
        // Are we the client?
        if (clientId == NetworkManager.Singleton.LocalClientId) OnLocalClientConnected?.Invoke(clientId);
        else OnClientConnected?.Invoke(clientId);

        if (NetworkManager.Singleton.IsServer) {
            LobbyPlayer player = Instantiate(lobbyPlayerPrefab).GetComponent<LobbyPlayer>();
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    private void HandleClientDisconnected(ulong clientId) {
        OnClientDisconnected?.Invoke(clientId);
    }

    private void HandleSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode) {
        if (sceneName != "Game") return;
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
