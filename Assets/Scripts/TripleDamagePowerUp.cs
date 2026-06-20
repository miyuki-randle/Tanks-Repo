using Unity.Netcode;
using UnityEngine;

public class TripleDamagePowerUp : PowerUp
{
    protected override void Activate(ulong playerID)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerID, out var client))
        {
            PlayerController pc = client.PlayerObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyTripleDamage();
            }
        }
    }
}
