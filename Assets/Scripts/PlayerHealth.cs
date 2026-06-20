using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private Slider healthBar;

    public NetworkVariable<int> Health = new NetworkVariable<int>(
        5, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        Health.Value = 5;
        healthBar.maxValue = Health.Value;
        healthBar.value = Health.Value;

        Health.OnValueChanged += OnHealthChanged;
    }

    private void OnHealthChanged(int previous, int current)
    {
        healthBar.value = current;
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        Health.Value -= amount;

        if (Health.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        GetComponent<NetworkObject>().Despawn();
    }

    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= OnHealthChanged;
    }

    public void ResetHealth(int value)
    {
        if (!IsServer) return;

        Health.Value = value;
        healthBar.value = value;
    }

}
