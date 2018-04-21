using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct OnPizzaIngredient : IComponentData
{
    public int IngedientType;
    public Pizza OnPizza;
}