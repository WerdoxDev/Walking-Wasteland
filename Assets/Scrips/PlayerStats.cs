using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour {
    public NetworkVariable<int> Health = new();
    public int MaxHealth;

    public event Action<int> OnHealthChanged;

    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(int health) {
        Health.Value = health;

        OnHealthChanged?.Invoke(Health.Value);
        if (Health.Value <= 0) Die();
    }

    public void Die() {
        Debug.Log("I am Dead :L");
    }
}
