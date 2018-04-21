using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerInput : IComponentData
{
    public float2 Move;
    public float2 PlayerHeading;
    public int didShoot;
    public float FireCooldown;

    public bool CoolDownFinished => FireCooldown <= 0.0 && math.length(PlayerHeading) > 0.5f;
    public bool CanFire => CoolDownFinished && didShoot == 0;
}