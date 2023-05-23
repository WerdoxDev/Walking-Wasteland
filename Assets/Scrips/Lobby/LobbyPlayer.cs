using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour {
    public NetworkVariable<LobbyPlayerData> Data = new();

    public override void OnNetworkSpawn() {
        Data.OnValueChanged += (LobbyPlayerData oldData, LobbyPlayerData newData) => {
            if (string.IsNullOrEmpty(oldData.Username.ToString()))
                UIManager.Instance.LobbyPanel.AddPlayer(newData, this, IsOwner);

            UIManager.Instance.LobbyPanel.UpdatePlayer(OwnerClientId, Data.Value);
        };

        if (!IsOwner && !string.IsNullOrEmpty(Data.Value.Username.ToString()))
            UIManager.Instance.LobbyPanel.AddPlayer(Data.Value, this, false);
        else if (IsOwner)
            SetDataServerRpc(new() {
                Username = string.IsNullOrEmpty(UIManager.Instance.Username) ? "Test" : UIManager.Instance.Username,
                Team = 0,
                Sprite = PlayerSpriteType.ManBlue,
                IsHost = LobbyManager.Instance.LobbyPlayers.Count == 0
            });
    }

    public void SetUsername(string username) {
        LobbyPlayerData newData = Data.Value;
        newData.Username = username;
        SetDataServerRpc(newData);
    }

    public void ChangeTeam(int increment) {
        int teamColorCount = UIManager.Instance.LobbyPanel.TeamColors.Length;
        int newTeam = (Data.Value.Team + increment + teamColorCount) % teamColorCount;

        LobbyPlayerData newData = Data.Value;
        newData.Team = (byte)newTeam;
        SetDataServerRpc(newData);
    }

    public void ChangeSprite(int increment) {
        int playerSpritesCount = GameManager.Instance.PlayerSprites.Length;
        int spriteIndex = GameManager.Instance.GetPlayerSpriteIndexByType(Data.Value.Sprite);
        int newPlayerSprite = (spriteIndex + increment + playerSpritesCount) % playerSpritesCount;

        LobbyPlayerData newData = Data.Value;
        newData.Sprite = GameManager.Instance.PlayerSprites[newPlayerSprite].Type;
        SetDataServerRpc(newData);
    }

    public void SetIsReady(bool isReady) {
        LobbyPlayerData newData = Data.Value;
        newData.IsReady = isReady;
        SetDataServerRpc(newData);
    }

    [ServerRpc]
    public void SetDataServerRpc(LobbyPlayerData newData) {
        Data.Value = newData;
    }
}

public struct LobbyPlayerData : IEquatable<LobbyPlayerData>, INetworkSerializable {
    public FixedString32Bytes Username;
    public byte Team;
    public PlayerSpriteType Sprite;
    public bool IsReady;
    public bool IsHost;

    public LobbyPlayerData(FixedString32Bytes username, byte team, PlayerSpriteType sprite, bool isReady, bool isHost) {
        Username = username;
        Team = team;
        Sprite = sprite;
        IsReady = isReady;
        IsHost = isHost;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref Username);
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref Sprite);
        serializer.SerializeValue(ref IsReady);
        serializer.SerializeValue(ref IsHost);
    }

    public bool Equals(LobbyPlayerData other) =>
        Username == other.Username &&
        Team == other.Team &&
        Sprite == other.Sprite &&
        IsReady == other.IsReady &&
        IsHost == other.IsHost;
}