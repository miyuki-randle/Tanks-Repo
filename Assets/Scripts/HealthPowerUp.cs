using Unity.Netcode;
using UnityEngine;

public class HealthPowerUp : PowerUp
{
    protected override void Activate(ulong playerID)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerID, out var client))
        {
            PlayerHealth hp = client.PlayerObject.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.ResetHealth(5);
            }
        }
    }
}