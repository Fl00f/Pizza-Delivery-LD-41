using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TimedLife : IComponentData
{
    public float TimeToLive;
}