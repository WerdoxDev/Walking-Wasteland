using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour {
    public int Damage;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!IsServer) return;
        if (other.TryGetComponent(out Player player)) {
            player.Stats.SetHealthServerRpc(player.Stats.Health.Value - Damage);
        }
    }
}
