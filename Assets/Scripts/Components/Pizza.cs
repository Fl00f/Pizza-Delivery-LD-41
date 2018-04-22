using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Pizza : IComponentData
{
    public int PizzaId;
    public int ExpectedCost;
    public int ActualCost;
}