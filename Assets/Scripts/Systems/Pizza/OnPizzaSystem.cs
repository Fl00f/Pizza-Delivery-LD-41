using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class OnPizzaSystem : ComponentSystem
{
    private struct PizzaAssemblyData
    {
        public int Length;
        public ComponentDataArray<Pizza> pizza;
        [ReadOnly] public SharedComponentDataArray<PizzaOrder> pizzaOrder;
    };

    [Inject] PizzaAssemblyData pizzaAssemblyData;
    ComponentGroup onPizzaIngredients;

    protected override void OnCreateManager(int capacity)
    {
        onPizzaIngredients = GetComponentGroup(typeof(OnPizzaIngredient), typeof(PizzaGroup));
    }

    protected override void OnUpdate()
    {
        for (var p = 0; p < pizzaAssemblyData.Length; p++)
        {
            List<int> ingredientTypes = new List<int>();
            onPizzaIngredients.SetFilter(new PizzaGroup { PizzaId = p });

            var length = onPizzaIngredients.CalculateLength();
            for (int i = 0; i < length; i++)
            {
                ingredientTypes.Add(i);
            }

            PizzaOrder pizzaOrder = pizzaAssemblyData.pizzaOrder[p];

            List<int> missingIngredients = pizzaOrder.IngredientType.Except<int>(ingredientTypes).ToList();
            List<int> extraIngredients = ingredientTypes.Except<int>(pizzaOrder.IngredientType).ToList();

            var expectedCost = pizzaOrder.IngredientType.Count * 10;
            var actualCost = expectedCost - missingIngredients.Count * 5 - extraIngredients.Count * 5;

            // TODO: Create pizza spawner and initialize only once.
            Pizza pizza = pizzaAssemblyData.pizza[p];
            pizza.ExpectedCost = expectedCost;
            pizza.ActualCost = actualCost;
            pizzaAssemblyData.pizza[p] = pizza;

            // Debug.Log("PIZZA " + pizza.PizzaId + ": " + pizza.ExpectedCost + " | " + pizza.ActualCost + " | " + "Missing " + String.Join("; ", missingIngredients) + " | " + "Extra " + String.Join("; ", extraIngredients));
        }
    }
}