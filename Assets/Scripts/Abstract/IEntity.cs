using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    int MaxHp { get; }
    int Hp { get; }
    bool Dead { get; }
    void Die();

    Transform Body { get; }

    System.Action OnDie { get; set; }
}
