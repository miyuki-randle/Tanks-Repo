using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public Transform CanonPivot;
    public float RotateSpeed = 45f;

    public Transform Tip;
    public GameObject ProjectilePrefab;
    public float ShootForce = 15f;

    private float currentAngle = 0f;

    [SerializeField] private Material[] materials;
    [SerializeField] private Vector3[] spawnPoints;

    private NetworkVariable<int> materialIndex = new NetworkVariable<int>();
    private NetworkVariable<Vector3> spawnPosition = new NetworkVariable<Vector3>();

    private NetworkVariable<float> canonAngle = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private bool IsMyTurn =>
        GameManager.Instance != null &&
        GameManager.Instance.CurrentTurnClient.Value == OwnerClientId;

    private NetworkVariable<bool> tripleDamage = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            int spawnId = (int)OwnerClientId % spawnPoints.Length;
            spawnPosition.Value = spawnPoints[spawnId];

            materialIndex.Value = (int)OwnerClientId % materials.Length;
        }
        transform.position = spawnPosition.Value;
        ApplyTeam();

        canonAngle.OnValueChanged += (oldVal, newVal) =>
        {
            CanonPivot.localRotation = Quaternion.Euler(0f, 0f, newVal);
        };
    }

    void Update()
    {
        if (!IsOwner || !IsMyTurn) return;
        AimCanon();

        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();
    }

    void ApplyTeam()
    {
        var renderers = GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            if (r.gameObject.CompareTag("Wheel"))
                continue;

            r.material = materials[materialIndex.Value];
        }
    }

    void AimCanon()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
        {
            currentAngle += -input * RotateSpeed * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle, -90f, 90f);
            canonAngle.Value = currentAngle;
            CanonPivot.localRotation = Quaternion.Euler(0f, 0f, canonAngle.Value);
        }
    }

    void Shoot()
    {
        if (!IsOwner) return;

        ShootServerRpc(CanonPivot.up, OwnerClientId);
        GameManager.Instance.EndTurnServerRpc();
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 dir, ulong owner)
    {
        int damage = tripleDamage.Value ? 3 : 1;

        tripleDamage.Value = false;

        GameObject proj = Instantiate(ProjectilePrefab, Tip.position, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        p.damage = damage;
        p.owner = owner;

        NetworkObject netObj = proj.GetComponent<NetworkObject>();
        netObj.Spawn();

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.velocity = dir.normalized * ShootForce;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyTripleDamageServerRpc()
    {
        tripleDamage.Value = true;
    }

    public void ApplyTripleDamage()
    {
        if (IsServer)
            tripleDamage.Value = true;
        else
            ApplyTripleDamageServerRpc();
    }

}
