using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PizzaCost : IComponentData
{
    public int OrderCost;
    public int ActualCost;
}