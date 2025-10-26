using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HerbDataView : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;

    [Header("Header")]
    public TMP_Text titleTH;
    public Image herbImage;

    [Header("Single Info Text")]
    public TMP_Text infoBlock;

    public void Bind(HerbDataSO h)
    {
        if (!h) return;

        if (titleTH) titleTH.text = h.nameTH;
        if (herbImage) herbImage.sprite = h.image;
        if (infoBlock) infoBlock.text = BuildInfoText(h);
    }

    public void Show(bool on) => panelRoot?.SetActive(on);

    private string BuildInfoText(HerbDataSO h)
    {
        var sb = new StringBuilder(512);
        var nl = "\n";

        sb.AppendLine($"ชื่อวิทยาศาสตร์ : {h.scientificName}");
        if (!string.IsNullOrWhiteSpace(h.family))
            sb.AppendLine($"วงศ์ : {h.family}");
        sb.AppendLine();

        AppendSection(sb, "สรรพคุณ", h.propertiesText, nl);
        AppendSection(sb, "การใช้งาน", h.usageText, nl);
        AppendSection(sb, "", h.finaldescriptionText, nl, includeTitle: false);

        return sb.ToString().TrimEnd();
    }

    private void AppendSection(StringBuilder sb, string title, string body, string nl, bool includeTitle = true)
    {
        if (string.IsNullOrWhiteSpace(body)) return;

        sb.Append(nl);
        if (includeTitle && !string.IsNullOrWhiteSpace(title))
            sb.AppendLine(title);

        sb.AppendLine(body.Trim());
        sb.Append(nl);
    }
}
