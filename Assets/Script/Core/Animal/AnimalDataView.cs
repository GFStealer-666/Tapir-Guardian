using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalDataView : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;

    [Header("Header")]
    public TMP_Text titleTH;
    [Header("Main Image")]
    public Image mainImage;

    [Header("Single Info Text")]
    public TMP_Text infoBlock;  // <-- one TMP to display everything

    [Header("Formatting")]
    public bool useLineSpacing = true;  // add blank lines between sections

    public void Bind(AnimalDataSO animal)
    {
        if (!animal) return;

        if (titleTH) titleTH.text = animal.commonNameTH;
        if (mainImage)
        {
            Color c = mainImage.color;
            c.a = 255;

            mainImage.color = c;
            mainImage.sprite = animal.realImage;
        }

        if (infoBlock)
            infoBlock.text = BuildInfoText(animal);
    }

    public void Show(bool on) => panelRoot?.SetActive(on);

    // ----------------------------------------------------------------------

    private string BuildInfoText(AnimalDataSO a)
    {
        var sb = new StringBuilder(512);
        var nl = "\n";

        sb.AppendLine($"ชื่อวิทยาศาสตร์ : {a.scientificName}");
        sb.AppendLine($"วงศ์ : {a.family}");
        AppendSection(sb, "ข้อมูลทั่วไป", a.generalInfo, nl);
        AppendSection(sb, "ลักษณะ", a.characteristics, nl);
        AppendSection(sb, "สถานะการอนุรักษ์", a.conservationStatus, nl);
        AppendSection(sb, "สาเหตุของการใกล้สูญพันธุ์", a.threats, nl);
        AppendSection(sb, "อาหาร", a.diet, nl);
        AppendSection(sb, "แหล่งอาศัย", a.habitats, nl, includeTrailingLine: false);

        return sb.ToString().TrimEnd();
    }

    private void AppendSection(StringBuilder sb, string title, string body, string nl, bool includeTrailingLine = true)
    {
        if (string.IsNullOrWhiteSpace(body)) return;

        // add header
        sb.AppendLine(title);

        // add section text
        sb.AppendLine(body.Trim());

        // extra spacing after section (optional)
        if (includeTrailingLine)
            sb.Append(nl);
    }
}
