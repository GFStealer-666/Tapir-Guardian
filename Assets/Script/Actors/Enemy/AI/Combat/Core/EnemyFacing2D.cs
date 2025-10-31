using UnityEngine;

public interface IFacing2D { Vector2 Forward { get; } int Sign { get; } }

[DisallowMultipleComponent]
public class EnemyFacing2D : MonoBehaviour, IFacing2D
{
    [SerializeField] private LinearMover mover;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField, Range(0f,0.3f)] private float deadzone = 0.05f;
    [SerializeField] private bool faceByVelocity = true;

    int sign = +1;

    void Reset(){ mover = GetComponent<LinearMover>(); sprite = GetComponentInChildren<SpriteRenderer>(true); }
    void Awake(){ if(!mover) mover = GetComponent<LinearMover>(); if(!sprite) sprite = GetComponentInChildren<SpriteRenderer>(true); }

    void Update(){
        if(faceByVelocity && mover){
            float x = mover.CurrentVelocity.x;
            if (x >  deadzone) sign = +1;
            if (x < -deadzone) sign = -1;
        }
        if (sprite) sprite.flipX = (sign < 0);
    }

    public void FaceByTargetX(float tx){
        sign = (tx >= transform.position.x) ? +1 : -1;
        if (sprite) sprite.flipX = (sign < 0);
    }

    public Vector2 Forward => sign >= 0 ? Vector2.right : Vector2.left;
    public int Sign => sign;
}
