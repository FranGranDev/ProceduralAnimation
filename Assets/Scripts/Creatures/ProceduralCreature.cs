using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class ProceduralCreature : MonoBehaviour
    {
        [SerializeField] private CreaturesData Data;

        public void Generate()
        {

        }
        
    }
    public struct LegData
    {
        Transform Base;
        Transform CenterPoint;
        Transform TargetPoint;
        Transform[] Bones;

        float Lenght;
    }

    public struct LegSettings
    {

    }
    public struct LegPartSettings
    {

    }
}