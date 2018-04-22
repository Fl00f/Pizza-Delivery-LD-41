using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

public struct PizzaSpawnData : ISharedComponentData
{
    public List<int> IngredientList;

    public Position2D Position;
}