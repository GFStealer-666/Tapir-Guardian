using UnityEngine;

public interface IAttackOriginProvider
{
    /// Returns the world-space origin to use for range checks (e.g., hand/weapon)
    Vector2 GetOrigin(Transform self);
}
