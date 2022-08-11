using UnityEngine;

public interface IMovement
{
    Transform Body { get; }

    public void Move(Vector3 direction, float rotation);
    public void Jump();
    public MoveState State { get; }
}
public enum MoveState { OnGround, Fly, Jumping };
