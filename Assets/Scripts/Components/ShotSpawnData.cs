using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

public struct ShotSpawnData : IComponentData
{
    public Shot Shot;
    public Position2D Position;
    public Heading2D Heading;
    public int Faction;
}