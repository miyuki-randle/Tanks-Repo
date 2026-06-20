using Unity.Netcode;
using UnityEngine;

public abstract class PowerUp : NetworkBehaviour
{
    public float hoverSpeed = 1f;
    public float hoverHeight = 0.25f;

    private Vector3 startPos;

    public override void OnNetworkSpawn()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!IsSpawned) return;

        float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateServerRpc(ulong playerID)
    {
        if (!IsServer) return;

        Activate(playerID);

        GetComponent<NetworkObject>().Despawn();
    }

    protected abstract void Activate(ulong playerID);
}
