// Assets/Scripts/AI/Debug/EnemyDebugGizmos.cs
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class EnemyDebugGizmos : MonoBehaviour
{
    [SerializeField] private EnemyController controller;   // auto if null
    [SerializeField] private MeleeAttackBehaviour melee;     // auto if melee enemy

    [Header("Colors")]
    [SerializeField] private Color sightColor = new Color(0.2f, 0.8f, 1f, 0.35f);
    [SerializeField] private Color fovEdgeColor = new Color(0.1f, 0.6f, 1f, 0.8f);
    [SerializeField] private Color meleeColor = new Color(1f, 0.3f, 0.2f, 0.4f);

    void OnValidate()
    {
        controller ??= GetComponent<EnemyController>();
    }

    void OnDrawGizmosSelected()
    {
        controller ??= GetComponent<EnemyController>();
        if (controller == null) return;

        // Draw sight radius
        float r = controller.SightRadius;   // expose getter below
        if (r > 0f)
        {
            DrawCircle(transform.position, r, sightColor);
        }

        // Draw FOV edges (as rays)
        float fov = controller.Fov;         // expose getter below
        if (fov > 0f)
        {
            Vector2 fwd = transform.right;
            float half = fov * 0.5f * Mathf.Deg2Rad;
            Vector2 left  = Rotate(fwd, +half);
            Vector2 right = Rotate(fwd, -half);
            Gizmos.color = fovEdgeColor;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + left  * r);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + right * r);
        }
    }

    private static Vector2 Rotate(Vector2 v, float rad)
    {
        float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }

    private static void DrawCircle(Vector3 center, float radius, Color color, int segments = 32)
    {
        Gizmos.color = color;
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float ang = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, Mathf.Sin(ang) * radius, 0f);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
