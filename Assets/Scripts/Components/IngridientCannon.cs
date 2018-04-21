using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

public struct IngridientCannon : IComponentData
{
    public int CannonOn;
    public float FireCooldown;

    public bool CoolDownFinished => FireCooldown <= 0.0;
    public bool CanFire => CoolDownFinished && CannonOn == 1;
}