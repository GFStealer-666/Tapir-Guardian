using UnityEngine;

[DisallowMultipleComponent]
public class EnemyRoot : MonoBehaviour
{
    public HealthComponent health;
    public IDamageable Idamageable;
    public LinearMover Mover;

    [Header("Target")]
    public Transform Target;

    void OnEnable()
    {
        health.OnDied += Died;
    }
    void OnDisable()
    {
        health.OnDied -= Died;
    }
    void Awake()
    {
        health = GetComponentInChildren<HealthComponent>();
        Idamageable = GetComponentInChildren<IDamageable>();
        Mover = GetComponentInChildren<LinearMover>();

        if (Mover == null) Debug.LogError("PlayerRoot2D: Imover2D missing", this);
        if (health == null) Debug.LogError("PlayerRoot2D: IHealth missing", this);

        if (Target == null)
        {
            var target = GameObject.FindGameObjectWithTag("Player");
            if (target) Target = target.transform;
        }
    }
    
    public void Died()
    {
        this.gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} EnemyRoot Died");
    }
}
