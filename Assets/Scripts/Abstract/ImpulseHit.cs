using UnityEngine;

public class ImpulseHit : Hit
{
    public Vector3 Impulse { get; set; }

    public ImpulseHit(int damage, Vector3 impulse) : base(damage)
    {
        Impulse = impulse;
    }

}