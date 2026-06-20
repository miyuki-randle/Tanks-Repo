using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public int damage = 1;
    public ulong owner;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // hit a player
        if (other.TryGetComponent<PlayerHealth>(out var player))
        {
            player.TakeDamage((int)damage);
            GetComponent<NetworkObject>().Despawn();
            return;
        }

        // hit a power-up
        if (other.TryGetComponent<PowerUp>(out var pu))
        {
            pu.ActivateServerRpc(owner);
            GetComponent<NetworkObject>().Despawn();
            return;
        }

        if (other.gameObject.tag == "Ground")
        {
            NetworkObject netObj = other.GetComponent<NetworkObject>();
            if (netObj.IsSpawned) netObj.Despawn();
            GetComponent<NetworkObject>().Despawn();
            return;
        }
    }


    // auto-despawn after 5 seconds
    private void Start()
    {
        if (IsServer)
            Invoke(nameof(DespawnBullet), 5f);
    }

    private void DespawnBullet()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}
