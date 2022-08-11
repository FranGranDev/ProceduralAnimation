using UnityEngine;

public interface IGadget
{
    public void OnActionStart(System.Action callback);
    public void OnActionEnd();
    public void ActionPoint(Vector3 point);
    public void ChangeGadget(bool right);
}
