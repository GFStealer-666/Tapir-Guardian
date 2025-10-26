using UnityEngine;

[DisallowMultipleComponent]
public class EnemyRoot : MonoBehaviour
{
    public IHealth Ihealth;
    public IDamageable Idamageable;
    public LinearMover Mover;

    [Header("Target")]
    public Transform Target; // usually the player

    void Awake()
    {
        Ihealth = GetComponentInChildren<IHealth>();
        Idamageable = GetComponentInChildren<IDamageable>();
        Mover = GetComponentInChildren<LinearMover>();

        if (Mover == null) Debug.LogError("PlayerRoot2D: Imover2D missing", this);
        if (Ihealth == null) Debug.LogError("PlayerRoot2D: IHealth missing", this);
        
        if (Target == null)
        {
            var target = GameObject.FindGameObjectWithTag("Player");
            if (target) Target = target.transform;
        }
    }
}
