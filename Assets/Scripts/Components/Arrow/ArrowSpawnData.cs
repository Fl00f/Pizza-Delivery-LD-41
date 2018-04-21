using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

public struct ArrowSpawnData : IComponentData
{
    public Position2D Position;
    public Heading2D Heading;
}