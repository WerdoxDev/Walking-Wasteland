using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour {

    [Header("General")]
    public InputReader InputReader;
    public Transform SpriteHolder;
    public SpriteRenderer StandSpriteRenderer;
    public SpriteRenderer ReloadSpriteRenderer;
    public SpriteRenderer PistolSpriteRenderer;
    public SpriteRenderer SilencedPistolSpriteRenderer;
    public SpriteRenderer MachineGunSpriteRenderer;
    public Camera Camera;

    [Header("Autoset Fields")]
    public PlayerMovement Movement;
    public PlayerSpriteData SpriteData;
    public PlayerStats Stats;
    public Rigidbody2D Rigidbody;
    public LobbyPlayerData MultiplayerData;

    [Header("States")]
    public Transform CurrentSprite;
    public bool CanMove;
    public bool CanShoot;
    public bool CanSprint;
    public bool CanInteract;

    private void Awake() {
        Movement = GetComponent<PlayerMovement>();
        SpriteData = GetComponent<PlayerSpriteData>();
        Stats = GetComponent<PlayerStats>();
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            Camera.gameObject.SetActive(false);
        }

        MultiplayerData = LobbyManager.Instance.GetLobbyPlayer(OwnerClientId).Data.Value;

        SpriteData.SetSprite(MultiplayerData.Sprite);
        SpriteData.SetSpriteState(PlayerSpriteState.Pistol);

        GameManager.Instance.PlayerSpawned(this, IsOwner);
    }

    public void SetKeybindsActive(bool isActive) {
        CanMove = CanInteract = CanSprint = CanSprint = isActive;
    }
}
