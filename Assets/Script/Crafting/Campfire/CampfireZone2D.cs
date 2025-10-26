// Assets/Scripts/Crafting/CampfireZone2D.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class CampfireZone2D : MonoBehaviour
{
    [Header("Who can use this campfire")]
    [SerializeField] private string playerTag = "Player";

    [Header("Crafting UI")]
    [SerializeField] private GameObject craftButton;           // top button you show/hide
    [SerializeField] private CraftingStationController craftingUI;  // your existing UI controller
    [SerializeField] private MonoBehaviour stationBehaviour;   // assign a component that implements ICraftingStation

    [Header("Audio")]
    [SerializeField] private AudioSource fireLoop;             // campfire loop (spatial blend 1.0 if 3D)
    [SerializeField] private float fadeDuration = 0.35f;       // seconds

    private ICraftingStation station;
    private readonly HashSet<GameObject> _inside = new();

    void Awake()
    {
        if (craftButton) craftButton.GetComponent<Button>().interactable = false;
        station = stationBehaviour as ICraftingStation;
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        _inside.Add(other.gameObject);

        if (craftButton) craftButton.GetComponent<Button>().interactable = true;

        // audio in
        if (fireLoop) AudioFader.Fade(this, fireLoop, 1f, fadeDuration);

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        _inside.Remove(other.gameObject);

        // If nobody left inside, hide UI and cancel jobs
        if (_inside.Count == 0)
        {
            if (craftButton) craftButton.GetComponent<Button>().interactable = false;

            if (craftingUI != null)
            {
                craftingUI.CancelAllJobs();      // ← cancels current crafting, if any
                craftingUI.ClearActiveStation(); // optional, so button won’t act on a stale station
            }

            // audio out
            if (fireLoop) AudioFader.Fade(this, fireLoop, 0f, fadeDuration);
        }
    }

    void OnDisable()
    {
        if (craftButton) craftButton.SetActive(false);
        craftingUI?.CancelAllJobs();
        craftingUI?.ClearActiveStation();

        if (fireLoop)
        {
            fireLoop.volume = 0f;
            fireLoop.Pause();
        }
        _inside.Clear();
    }
}
