using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class BlockComponent : MonoBehaviour, IBlock
{
     [Header("Timer")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float perfectWindowDuration = 0.15f;
    [SerializeField] private float cooldown = 5f;
    [Header("Block Effect")]
    [Range(0,1f)]
    [SerializeField] private float normalBlockMultiplier = 0.5f;
    [SerializeField] private float blockMovespeedMultiplier = 0.5f;

    [Header("Debug")]
    [SerializeField] private float nextCooldownTime;
    [SerializeField] private float blockEndTime;
    [SerializeField] private float perfectEndTime;
    private float Now => Time.time;
    public bool IsBlocking => Now <= blockEndTime;
    public bool IsPerfectBlocking => Now < perfectEndTime;
    public bool IsOnCooldown => !IsBlocking && Now < nextCooldownTime;
    public float BlockMultiplier => IsPerfectBlocking ?
    0f : (IsBlocking ? Mathf.Clamp01(0.5f) : 1f);
    // if perfect block damage deal = 0f;
    // if normal block = 0.5
    [SerializeField] private SpeedModifierStack speedStack;
    public float MoveSpeedModifier => IsBlocking ? blockMovespeedMultiplier : 1f;
    void Awake()
    {
        speedStack = GetComponentInParent<SpeedModifierStack>() ?? GetComponent<SpeedModifierStack>();
    }
    void Update()
    {
        if (!IsBlocking) speedStack?.RemoveModifier(this);
    }
    public bool TryBlock()
    {
        Debug.Log("Try Blocking");
        if (IsBlocking || IsOnCooldown) return false;
        // If currently player is blocking or it in cooldown will return
        Debug.Log("Actually Blocking");
        float now = Now;
        float pw = Mathf.Clamp(perfectWindowDuration, 0f, duration); // try to make sure it not blocking longer than the duration it should be

        speedStack.SetModifier(this, Mathf.Clamp(blockMovespeedMultiplier, 0f, 1f), "Block");
        nextCooldownTime = now + cooldown;
        blockEndTime = now + duration;
        perfectEndTime = now + pw;
        return true;
    }
    void OnDisable() => speedStack?.RemoveModifier(this);
}
