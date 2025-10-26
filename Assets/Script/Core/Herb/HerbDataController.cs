using System.Linq;
using UnityEngine;

public class HerbDataController : MonoBehaviour
{
    [Header("Data")]
    public HerbDatabaseSO database;

    [Header("UI Panel")]
    public HerbDataPanel panel;

    [Header("List UI (Left Column)")]
    public Transform listParent;
    public HerbListItemButton listItemPrefab;

    private HerbCodexSession _herbSession;
    private System.Collections.Generic.Dictionary<string, HerbDataSO> _byId;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _byId = database.herbs
            .Where(h => h && !string.IsNullOrWhiteSpace(h.id))
            .ToDictionary(h => h.id, h => h);

        if (panel)
            panel.Hide();
    }

    void OnEnable()
    {
        _herbSession = HerbCodexSession.Instance;
        if (_herbSession != null)
            _herbSession.OnChanged += RebuildList;
        RebuildList();
    }

    void OnDisable()
    {
        if (_herbSession != null)
            _herbSession.OnChanged -= RebuildList;
    }

    public void SelectHerb(string id)
    {
        if (_herbSession == null) return;
        if (!_herbSession.IsUnlocked(id)) return;

        if (_byId.TryGetValue(id, out var h))
            panel.Show(h);
    }

    public void OpenCodex()
    {
        if (panel)
            panel.Show(null);
        RebuildList();
    }

    public void CloseCodex() => panel?.Hide();

    public void Unlock(HerbDataSO h)
    {
        if (!h || string.IsNullOrWhiteSpace(h.id)) return;
        if (HerbCodexSession.Instance != null)
            HerbCodexSession.Instance.Unlock(h.id);
    }

    private void RebuildList()
    {
        for (int i = listParent.childCount - 1; i >= 0; i--)
            Destroy(listParent.GetChild(i).gameObject);

        bool hasSession = (_herbSession != null);
        var items = _byId.Values.OrderBy(h => h.nameTH);

        foreach (var h in items)
        {
            var btn = Instantiate(listItemPrefab, listParent);
            btn.controller = this;
            btn.herbId = h.id;

            bool isLocked = !hasSession || !_herbSession.IsUnlocked(h.id);
            btn.Setup(h.nameTH, h.image);
            btn.SetLocked(isLocked);
        }
    }
    
}
