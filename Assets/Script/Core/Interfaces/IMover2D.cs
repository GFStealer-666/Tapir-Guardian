using System.Numerics;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;

public interface IMover2D
{
    float MoveSpeed { get; }
    Vector2 CurrentVelocity { get; }
    void Move(Vector2 dir);
    void Jump(); // optional
}
