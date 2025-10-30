using UnityEngine;

/// <summary>
/// Listens to HealthComponent death event and spawns an item drop prefab.
/// Works with any entity that has HealthComponent.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(HealthComponent))]
public class ItemDropOnDeath : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private GameObject dropPrefab;   // prefab with ItemPickup.cs
    [SerializeField] private ItemSO itemData;
    [Tooltip("If applicable, how many items to give in each pickup")]      
    [SerializeField] private int minAmount = 1;
    [SerializeField] private int maxAmount = 1;
    [SerializeField] private int itemCount = 5;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 1.0f; // 1 = always, 0.5 = 50%

    [Header("Drop Physics")]
    [SerializeField] private float scatterForce = 2.0f;
    [SerializeField] private float upwardForce = 1.0f;

    private HealthComponent _health;

    private void Awake()
    {
        _health = GetComponent<HealthComponent>();
        if (_health == null)
            Debug.LogWarning($"[ItemDropOnDeath] No HealthComponent found on {name}");
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Roll chance
        if (Random.value > dropChance || dropPrefab == null) return;

        // How many items to drop
        int count = Random.Range(minAmount, maxAmount + 1);

        for (int i = 0; i < count; i++)
        {
            var drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            var itemPickup = drop.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.Configure(itemData, itemCount);
            }

            // Add random scatter
            var rb = drop.GetComponent<Rigidbody2D>();
            if (rb)
            {
                Vector2 dir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(dir * scatterForce + Vector2.up * upwardForce, ForceMode2D.Impulse);
            }
        }
    }
}
