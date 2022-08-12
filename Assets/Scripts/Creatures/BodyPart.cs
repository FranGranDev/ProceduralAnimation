using System;
using UnityEngine;

public class BodyPart : MonoBehaviour, IEntity
{
    private IEntity parent;

    public int MaxHp => parent.MaxHp;
    public int Hp => parent.Hp;

    public void GetHit(Hit hit)
    {
        parent.GetHit(hit);
    }
    public void GetHit(ImpulseHit hit)
    {
        parent.GetHit(hit);
    }

    public bool Dead => parent.Dead;
    public Action OnDie { get => parent.OnDie; set => parent.OnDie = value; }
    public void Die()
    {
        parent.Die();
    }

    public Transform Body => parent.Body;

    public void Init(IEntity parent)
    {
        this.parent = parent;
    }

    public bool Compare(IEntity other)
    {
        return parent.Compare(other);
    }
}
