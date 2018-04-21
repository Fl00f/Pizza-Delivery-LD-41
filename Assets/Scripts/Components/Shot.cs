using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Shot : IComponentData
{
    public float TimeToLive;
    public float Energy;
}