using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OnPizzaSystem : ComponentSystem
{
    ComponentGroup onPizzaIngredients;

    protected override void OnCreateManager(int capacity)
    {
        onPizzaIngredients = GetComponentGroup(typeof(OnPizzaIngredient), typeof(PizzaGroup));
    }

    protected override void OnUpdate()
    {
        for (var p = 0; p < 2; p++)
        {
            onPizzaIngredients.SetFilter(new PizzaGroup { PizzaId = p });

            var length = onPizzaIngredients.CalculateLength();
            for (int i = 0; i < length; i++)
            {
                Debug.Log("Pizza " + p + ": " +
                    onPizzaIngredients.GetComponentDataArray<OnPizzaIngredient>()[i].IngedientType);
            }
            Debug.Log("*****");
        }
    }
}