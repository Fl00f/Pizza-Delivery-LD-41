using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct IngredientList : ISharedComponentData
{
    public List<int> Value;
}