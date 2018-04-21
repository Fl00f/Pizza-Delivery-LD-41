using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OnPizzaSystem : ComponentSystem
{
    private struct IngredientsOnPizzaData
    {
        public ComponentDataArray<OnPizzaIngredient> onPizzaIngredients;
    }

    [Inject] private IngredientsOnPizzaData onPizzaIngredientsData;

    protected override void OnUpdate()
    {
        //just usign this to see the entities in the debugger
    }
}