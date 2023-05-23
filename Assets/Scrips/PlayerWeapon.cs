using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : NetworkBehaviour {
    public NetworkVariable<WeaponType> CurrentWeaponType = new();

    [Header("General")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float bulletKeepAlive;
    [SerializeField] private Weapon pistol;
    [SerializeField] private Weapon silencedPistol;
    [SerializeField] private Weapon machineGun;

    private Player _player;
    private bool _isReloading;
    private bool _isShooting;
    private bool _canShoot;
    private int weaponIndex;

    private IEnumerator reloadCoroutine;
    private IEnumerator shootCoroutine;

    private void Awake() {
        _player = GetComponent<Player>();
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            enabled = false;

            CurrentWeaponType.OnValueChanged += (oldType, newType) => {
                ChangeWeapon(newType);
            };
        }
        else {
            ChangeWeaponServerRpc(WeaponType.Pistol);
            ChangeWeapon(WeaponType.Pistol);
            SetInputEvents(true);
            _canShoot = true;
        }
    }

    public void ChangeWeapon(WeaponType newType) {
        if (newType == WeaponType.Pistol) _player.SpriteData.SetSpriteState(PlayerSpriteState.Pistol);
        if (newType == WeaponType.SilencedPistol) _player.SpriteData.SetSpriteState(PlayerSpriteState.SilencedPistol);
        if (newType == WeaponType.MachineGun) _player.SpriteData.SetSpriteState(PlayerSpriteState.MachineGun);


        Weapon weapon = GetWeaponByType(newType);
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        if (shootCoroutine != null) StopCoroutine(shootCoroutine);
        _isReloading = false;
        _isShooting = false;
        _canShoot = !_isReloading && (weapon.TotalAmmo > 0 || weapon.CurrentAmmo > 0);
    }

    [ServerRpc]
    public void ChangeWeaponServerRpc(WeaponType newType) {
        CurrentWeaponType.Value = newType;
    }

    [ServerRpc]
    public void SpawnBulletServerRpc(Vector2 position, int damage, float speed, float spread) {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.LookRotation(Vector3.forward, -_player.SpriteHolder.right));
        bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);

        bullet.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(spread, -spread));
        bullet.GetComponent<Bullet>().Damage = damage;
        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * speed, ForceMode2D.Impulse);
        //SpawnBulletClientRpc(position, damage, speed, spread);
        Destroy(bullet, bulletKeepAlive);
    }

    //[ClientRpc]
    //private void SpawnBulletClientRpc(Vector2 position, int damage, float speed, float spread) {
    //}

    private void Shoot(Weapon weapon) {
        if (!_canShoot || _isReloading) return;
        shootCoroutine = ShootEnumerator(weapon);
        StartCoroutine(shootCoroutine);
    }

    private void Reload(Weapon weapon) {
        if (weapon.TotalAmmo <= 0 || weapon.CurrentAmmo == weapon.ClipSize || weapon.CurrentAmmo == weapon.TotalAmmo) return;
        reloadCoroutine = ReloadEnumerator(weapon);
        StartCoroutine(reloadCoroutine);
    }

    private Weapon GetWeaponByType(WeaponType type) {
        if (type == WeaponType.Pistol) return pistol;
        if (type == WeaponType.SilencedPistol) return silencedPistol;
        if (type == WeaponType.MachineGun) return machineGun;
        return null;
    }

    private IEnumerator ReloadEnumerator(Weapon weapon) {
        _isReloading = true;
        _canShoot = false;

        yield return new WaitForSeconds(weapon.ReloadTime);

        if (weapon.TotalAmmo - (weapon.ClipSize - weapon.CurrentAmmo) < 0) {
            weapon.CurrentAmmo += weapon.TotalAmmo;
            weapon.TotalAmmo = 0;
        }
        else {
            weapon.TotalAmmo -= weapon.ClipSize - weapon.CurrentAmmo;
            weapon.CurrentAmmo = weapon.ClipSize;
        }

        _canShoot = true;
        _isReloading = false;
    }

    private IEnumerator ShootEnumerator(Weapon weapon) {
        _isShooting = true;
        while (_isShooting) {
            if (!_canShoot) yield break;

            weapon.CurrentAmmo -= 1;
            SpawnBulletServerRpc(shootPosition.position, weapon.Damage, weapon.BulletSpeed, weapon.Spread);

            if (weapon.CurrentAmmo <= 0 && weapon.TotalAmmo > 0) {
                reloadCoroutine = ReloadEnumerator(weapon);
                StartCoroutine(reloadCoroutine);
                yield return new WaitWhile(() => _isReloading);
            }
            else if (weapon.CurrentAmmo <= 0 && weapon.TotalAmmo <= 0) {
                _canShoot = false;
                _isShooting = false;
            }

            _canShoot = false;
            yield return new WaitForSeconds(1f / weapon.FireRate);
            if (!_isReloading && weapon.CurrentAmmo > 0) _canShoot = true;
        }
    }

    private void OnDisable() => SetInputEvents(false);

    private void SetInputEvents(bool enabled) {
        void OnButtonEvent(ButtonType type, bool performed) {
            if (!performed && type == ButtonType.Shoot) _isShooting = false;
            if (performed) {
                if (type == ButtonType.Shoot) Shoot(GetWeaponByType(CurrentWeaponType.Value));
                else if (type == ButtonType.Reload) Reload(GetWeaponByType(CurrentWeaponType.Value));
            }
        }

        void OnWeaponChange(int weaponIncrement) {
            weaponIndex += weaponIncrement;
            if (weaponIndex > 2) weaponIndex = 0;
            if (weaponIndex < 0) weaponIndex = 2;

            if (weaponIndex == 0) {
                ChangeWeapon(WeaponType.Pistol);
                ChangeWeaponServerRpc(WeaponType.Pistol);
            }
            else if (weaponIndex == 1) {
                ChangeWeapon(WeaponType.SilencedPistol);
                ChangeWeaponServerRpc(WeaponType.SilencedPistol);
            }
            else if (weaponIndex == 2) {
                ChangeWeapon(WeaponType.MachineGun);
                ChangeWeaponServerRpc(WeaponType.MachineGun);
            }
        }

        if (enabled) {
            _player.InputReader.ButtonEvent += OnButtonEvent;
            _player.InputReader.OnWeaponChange += OnWeaponChange;
        }
        else {
            _player.InputReader.ButtonEvent -= OnButtonEvent;
            _player.InputReader.OnWeaponChange -= OnWeaponChange;
        }
    }
}

[Serializable]
public class Weapon {
    public WeaponType Type;

    public int Damage;
    public float Spread;
    public float BulletSpeed;
    public int TotalAmmo;
    public int ClipSize;
    public int CurrentAmmo;
    public float FireRate;
    public float ReloadTime;
}

public enum WeaponType {
    None,
    Pistol,
    SilencedPistol,
    MachineGun
}
