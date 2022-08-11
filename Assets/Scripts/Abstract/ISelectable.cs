using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISelectable
{
    void OnSendAccept();
    void Accept(ISelectable other);
    void Accept(List<ISelectable> others);


    Transform transform { get; }
    bool Selected { get; set; }

    void MoveTo(Vector3 position);
    void Attack(IEntity entity);
    void Attack(List<IEntity> entitis);

    void Add(List<ISelectable> units);
    void Clear();

    System.Action OnDisable { get; set; }
}
