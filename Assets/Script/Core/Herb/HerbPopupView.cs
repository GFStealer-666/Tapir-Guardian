using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HerbPopupView : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;

    [Header("Header")]
    public TMP_Text herbNameTH;   // ว่านธรณีเย็น

    [Header("Image")]
    public Image herbImage;

    [Header("Single Text Block (RIGHT)")]
    public TMP_Text infoBlock;    // <- ONE TMP_Text to render everything

    [Header("Formatting")]
    [Tooltip("Use • bullets for list items; if false, joins with plain new lines.")]
    public bool useBullets = true;
    public void Bind(HerbDataSO h)
    {
        if (!h) return;

        if (herbNameTH) herbNameTH.text = h.nameTH;
        if (herbImage)  herbImage.sprite = h.image;

        // Build the info block
        infoBlock.text = h.finaldescriptionText;
    }

    public void Show(bool on) => panelRoot?.SetActive(on);

}
