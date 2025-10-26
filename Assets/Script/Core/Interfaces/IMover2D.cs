using System.Numerics;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;

public interface IMover2D
{
    float MoveSpeed { get; }
    
    void Move(Vector2 dir);
    void Jump(); // optional
}
