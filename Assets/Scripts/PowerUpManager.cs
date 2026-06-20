using Unity.Netcode;
using UnityEngine;

public class PowerUpManager : NetworkBehaviour
{
    public GameObject healthPowerUpPrefab;
    public GameObject tripleDamagePowerUpPrefab;

    public Vector2 spawnBoundsMin;
    public Vector2 spawnBoundsMax;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnPowerUp(healthPowerUpPrefab);
        SpawnPowerUp(tripleDamagePowerUpPrefab);
    }

    private void SpawnPowerUp(GameObject prefab)
    {
        Vector3 pos = new Vector3(
            Random.Range(spawnBoundsMin.x, spawnBoundsMax.x),
            0,
            Random.Range(spawnBoundsMax.y, spawnBoundsMax.y)
        );

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
    }
}
