using System.Collections;
using UnityEngine;


public class TapirSequence : MonoBehaviour
{
    [Header("References")]
    public AnimalAnimation runner;        // Assign on the same root
    public Transform hangAnchor;          // Where it hangs initially
    public GameObject parent;             // The object you want to move down
    [SerializeField] private Animator visualAnimator; // <-- assign the CHILD animator here

    [Header("Animation Settings")]
    [SerializeField] private float fallHeight = 3.0f;
    [SerializeField] private float fallDuration = 0.6f;
    [SerializeField] private string fallTrigger = "FallOff";

    bool _started;

    void Reset()
    {
        runner = GetComponent<AnimalAnimation>();
        visualAnimator = GetComponentInChildren<Animator>(true);
    }

    void Awake()
    {
        if (!runner) runner = GetComponent<AnimalAnimation>();
        if (!visualAnimator) visualAnimator = GetComponentInChildren<Animator>(true);

        if (hangAnchor) transform.position = hangAnchor.position;
    }

    public void OnInteract()
    {
        if (_started) return;
        _started = true;

        if (!visualAnimator)
        {
            Debug.LogError("[TapirSequence] No visualAnimator assigned/found.");
            return;
        }

        // 1) Trigger fall animation
        visualAnimator.ResetTrigger(fallTrigger);
        visualAnimator.SetTrigger(fallTrigger);

        // 2) Move parent down smoothly
        StartCoroutine(MoveParentDownSmooth());
    }

    private IEnumerator MoveParentDownSmooth()
    {
        if (!parent) yield break;

        Vector3 start = parent.transform.position;
        Vector3 target = start + Vector3.down * fallHeight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, fallDuration);
            parent.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }

    // Call this via an Animation Event on the LAST frame of the FallOff clip
    public void OnFallLanded()
    {
        if (!runner)
        {
            Debug.LogError("[TapirSequence] Runner not assigned.");
            return;
        }
        runner.TriggerEscape();
    }

    [ContextMenu("▶ Test Interaction")]
    public void TestInteract()
    {
        Debug.Log("[TapirSequence] Context Menu Triggered (Play Mode)!");
        OnInteract();
    }
}
