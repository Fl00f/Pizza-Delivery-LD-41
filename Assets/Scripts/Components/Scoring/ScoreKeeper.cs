using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ScoreKeeper : IComponentData
{
    public int Score;
}