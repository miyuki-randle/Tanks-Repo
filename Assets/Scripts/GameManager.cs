using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public GameObject cube;

    // which client ID is allowed to shoot right now
    public NetworkVariable<ulong> CurrentTurnClient =
        new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone,
                                      NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var players = NetworkManager.Singleton.ConnectedClientsIds;

            if (players.Count > 0)
                CurrentTurnClient.Value = players[0]; // first player starts

            SpawnGround();
        }
    }

    void SpawnGround()
    {
        for (int l = 0; l < 1; l++)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    Vector3 position = new Vector3(-20,-11,0) + new Vector3(x * 2,y * 2,l * 2);
                    GameObject obj = Instantiate(cube, position, Quaternion.identity, transform);
                    NetworkObject netObj = obj.GetComponent<NetworkObject>();
                    netObj.Spawn();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        var players = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

        if (players.Count < 2) return;

        int index = players.IndexOf(CurrentTurnClient.Value);
        int next = (index + 1) % players.Count;

        CurrentTurnClient.Value = players[next];
    }
}
