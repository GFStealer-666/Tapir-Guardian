using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalDataController : MonoBehaviour
{
    [Header("Data")]
    public AnimalDatabaseSO database;

    [Header("UI Panel")]
    public AnimalDataPanel panel;

    [Header("List UI (Left Column)")]
    public Transform listParent;                 // VerticalLayoutGroup content
    public AnimalListItemButton listItemPrefab;  // prefab with label, lock icon, etc.

    private Dictionary<string, AnimalDataSO> _byId;
    private AnimalCodexSession _animalSession;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  // stays alive for entire session

        // build lookup by id (so we can go id -> AnimalDataSO fast)
        _byId = database.animals
            .Where(a => a && !string.IsNullOrWhiteSpace(a.id))
            .ToDictionary(a => a.id, a => a);

        if (panel)
            panel.Hide();
    }

    void OnEnable()
    {
        // grab / cache session
        _animalSession = AnimalCodexSession.Instance;

        // listen to changes in unlock state
        if (_animalSession != null)
            _animalSession.OnChanged += RebuildList;

        RebuildList();
    }

    void OnDisable()
    {
        if (_animalSession != null)
            _animalSession.OnChanged -= RebuildList;
    }

    // player clicked an entry in the list
    public void SelectAnimal(string id)
    {
        if (_animalSession == null) return;
        if (!_animalSession.IsUnlocked(id)) return;

        if (_byId.TryGetValue(id, out var data))
        {
            panel.Show(data);
        }
    }

    // open/close codex panel
    public void OpenCodex()
    {
        // optional: either show last viewed animal OR just make sure panel is visible
        // safest: just ensure panel is visible and wait for player to click list
        if (panel)
        {
            panel.Show(null); // you'll update Show() to tolerate null safely (next section)
        }

        // also rebuild in case unlocks changed while closed
        RebuildList();
    }

    public void CloseCodex()
    {
        if (panel)
            panel.Hide();
    }

    // Compatibility for old code that still calls controller.Unlock(...)
    public void Unlock(AnimalDataSO a)
    {
        if (!a || string.IsNullOrWhiteSpace(a.id)) return;

        // send unlock to the real source of truth (session)
        if (AnimalCodexSession.Instance != null)
        {
            AnimalCodexSession.Instance.Unlock(a.id);
            // RebuildList() will be called by OnChanged anyway
        }
        else
        {
            Debug.LogWarning("[AnimalDataController] No AnimalCodexSession found, cannot unlock globally.");
        }
    }

    // rebuilds the left list UI (lock state, text, interactable etc.)
    private void RebuildList()
    {
        // clear old
        for (int i = listParent.childCount - 1; i >= 0; i--)
            Destroy(listParent.GetChild(i).gameObject);

        bool hasSession = (_animalSession != null);
        var items = _byId.Values.OrderBy(a => a.commonNameTH);

        foreach (var a in items)
        {
            var btn = Instantiate(listItemPrefab, listParent);
            btn.controller = this;
            btn.animalId = a.id;

            bool isLocked = !hasSession || !_animalSession.IsUnlocked(a.id);
            btn.Setup(a.commonNameTH, a.image);
            btn.SetLocked(isLocked);
        }
    }
}
