using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public bool Compare(IEntity other);

    int MaxHp { get; }
    int Hp { get; }

    bool Dead { get; }
    void Die();
    System.Action OnDie { get; set; }

    void GetHit(Hit hit);
    void GetHit(ImpulseHit hit);

    Transform Body { get; }
}
