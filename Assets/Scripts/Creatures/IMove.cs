using UnityEngine;

public interface IMove
{
    public void Move(Vector3 direction, float rotation);
    public void Jump();
    public void OnActionStart();
    public void OnActionEnd();
    public void ActionPoint(Vector3 point);
    public void ChangeGadget(bool right);
    public MoveState State { get; }
}
public enum MoveState { OnGround, Fly, Jumping };
