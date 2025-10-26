using UnityEngine;

public class PerceptionComponent : MonoBehaviour
{
    public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacleMask)
    {
        var dir = (to - from);
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;
        dir /= dist;
        return !Physics2D.Raycast(from, dir, dist, obstacleMask);
    }

    public static bool InFOV(Vector3 fromFwd, Vector3 toDir, float fovDeg)
    {
        if (toDir.sqrMagnitude < 0.0001f) return true;
        toDir.Normalize();
        var fwd = fromFwd.normalized;
        float angle = Vector2.Angle(fwd, toDir);
        return angle <= (fovDeg * 0.5f);
    }
}
