using UnityEngine;

[DisallowMultipleComponent]
public class PlayerRoot : MonoBehaviour
{
    public ILevel Ilevel;
    public IStat Istat;
    public IHealth Ihealth;
    public IDamageable Idamageable;
    public IMover2D Imover;
    public IBlock Iblock;

    void Awake()
    {
        Ilevel = GetComponent<ILevel>();
        Istat = GetComponent<IStat>();
        Ihealth = GetComponentInChildren<IHealth>();
        Idamageable = GetComponent<IDamageable>();
        Imover = GetComponent<IMover2D>();
        Iblock = GetComponent<IBlock>();

        if (Imover == null) Debug.LogError("PlayerRoot2D: Imover2D missing", this);
        if (Ihealth == null) Debug.LogError("PlayerRoot2D: IHealth missing", this);

    }
}
