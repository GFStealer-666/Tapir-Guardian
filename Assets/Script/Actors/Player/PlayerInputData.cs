// InputSnapshot.cs
using UnityEngine;

public struct InputSnapshot
{
    public Vector2 Move;
    public bool JumpPressed;
    public bool BlockPressed;
    public bool ShootPressed;
    public bool ShootHeld;

    public bool InteractPressed;   // <-- NEW (E key)
    public int HotbarPressedIndex; // you already had this in PlayerBrain
}
