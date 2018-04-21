using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

public struct IngridientSpawnData : IComponentData
{
    public int IngridientType;
    public Position2D spawnPosition;
    public Heading2D spawnHeading;
}