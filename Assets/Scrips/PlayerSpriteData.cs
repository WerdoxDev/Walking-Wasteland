using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpriteData : MonoBehaviour {
    [Header("General")]
    [SerializeField] private PlayerSpriteType currentType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Player _player;

    private void Awake() {
        _player = GetComponent<Player>();
    }

    public void SetSprite(PlayerSpriteType newType) {
        currentType = newType;
        PlayerSprite playerSprite = GameManager.Instance.GetPlayerSpriteByType(newType);
        _player.StandSpriteRenderer.sprite = playerSprite.Stand;
        _player.ReloadSpriteRenderer.sprite = playerSprite.Reload;
        _player.PistolSpriteRenderer.sprite = playerSprite.Pistol;
        _player.SilencedPistolSpriteRenderer.sprite = playerSprite.SilencedPistol;
        _player.MachineGunSpriteRenderer.sprite = playerSprite.MachineGun;
    }

    public void SetSpriteState(PlayerSpriteState newState) {
        _player.StandSpriteRenderer.gameObject.SetActive(false);
        _player.ReloadSpriteRenderer.gameObject.SetActive(false);
        _player.PistolSpriteRenderer.gameObject.SetActive(false);
        _player.SilencedPistolSpriteRenderer.gameObject.SetActive(false);
        _player.MachineGunSpriteRenderer.gameObject.SetActive(false);

        if (newState == PlayerSpriteState.Stand) _player.StandSpriteRenderer.gameObject.SetActive(true);
        if (newState == PlayerSpriteState.Reload) _player.ReloadSpriteRenderer.gameObject.SetActive(true);
        if (newState == PlayerSpriteState.Pistol) _player.PistolSpriteRenderer.gameObject.SetActive(true);
        if (newState == PlayerSpriteState.SilencedPistol) _player.SilencedPistolSpriteRenderer.gameObject.SetActive(true);
        if (newState == PlayerSpriteState.MachineGun) _player.MachineGunSpriteRenderer.gameObject.SetActive(true);

        _player.CurrentSprite = GetCurrentActiveSprite().transform;
    }

    public SpriteRenderer GetCurrentActiveSprite() {
        if (_player.StandSpriteRenderer.gameObject.activeSelf) return _player.StandSpriteRenderer;
        if (_player.ReloadSpriteRenderer.gameObject.activeSelf) return _player.ReloadSpriteRenderer;
        if (_player.PistolSpriteRenderer.gameObject.activeSelf) return _player.PistolSpriteRenderer;
        if (_player.SilencedPistolSpriteRenderer.gameObject.activeSelf) return _player.SilencedPistolSpriteRenderer;
        if (_player.MachineGunSpriteRenderer.gameObject.activeSelf) return _player.MachineGunSpriteRenderer;
        return null;
    }
}

[Serializable]
public struct PlayerSprite {
    public PlayerSpriteType Type;

    public Sprite Stand;
    public Sprite Pistol;
    public Sprite SilencedPistol;
    public Sprite MachineGun;
    public Sprite Reload;
}

public enum PlayerSpriteType {
    Hitman,
    ManBlue,
    ManBrown,
    WomanGreen,
    ManSoldier,
    ManOld
}

public enum PlayerSpriteState {
    Stand,
    Reload,
    Pistol,
    SilencedPistol,
    MachineGun
}
