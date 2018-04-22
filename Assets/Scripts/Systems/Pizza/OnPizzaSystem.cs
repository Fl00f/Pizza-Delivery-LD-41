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
        public ComponentDataArray<Pizza> Pizza;
        [ReadOnly] public SharedComponentDataArray<PizzaGroup> PizzaGroup;
        [ReadOnly] public SharedComponentDataArray<PizzaOrder> PizzaOrder;
    };

    [Inject] PizzaAssemblyData pizzaAssemblyData;
    ComponentGroup onPizzaIngredients;

    protected override void OnCreateManager(int capacity)
    {
        onPizzaIngredients = GetComponentGroup(typeof(Ingredient), typeof(PizzaGroup));
    }

    protected override void OnUpdate()
    {
        for (var p = 0; p < pizzaAssemblyData.Length; p++)
        {
            List<int> currentIngredients = new List<int>();
            onPizzaIngredients.SetFilter(pizzaAssemblyData.PizzaGroup[p]);

            var length = onPizzaIngredients.CalculateLength();
            for (int i = 0; i < length; i++)
            {
                currentIngredients.Add(onPizzaIngredients.GetComponentDataArray<Ingredient>()[i].IngredientType);
            }

            PizzaOrder pizzaOrder = pizzaAssemblyData.PizzaOrder[p];

            List<int> missingIngredients = pizzaOrder.IngredientType.Except<int>(currentIngredients).ToList();
            List<int> extraIngredients = currentIngredients.Except<int>(pizzaOrder.IngredientType).ToList();

            var expectedCost = pizzaOrder.IngredientType.Count * 10;
            var actualCost = expectedCost - missingIngredients.Count * 5 - extraIngredients.Count * 5;

            // TODO: Create pizza spawner and initialize only once.
            Pizza pizza = pizzaAssemblyData.Pizza[p];
            pizza.ExpectedCost = expectedCost; // TODO: This should be a property of the PizzaOrder.
            pizza.ActualCost = actualCost;
            pizzaAssemblyData.Pizza[p] = pizza;

            PizzaGroup pizzaGroup = pizzaAssemblyData.PizzaGroup[p];

            if (missingIngredients.Count == 0) {
                // Pizza is done.
                Debug.Log("PIZZA DONE - " + pizzaGroup.PizzaId + ": " + pizza.ExpectedCost + " | " + pizza.ActualCost + " | " + "Missing " + String.Join("; ", missingIngredients) + " | " + "Extra " + String.Join("; ", extraIngredients));

                // Update global score.

                // Delete this Pizza.
                // TODO: Move to a pizza destroyer system.
                //for (int i = 0; i < length; i++) {
                //    PostUpdateCommands.DestroyEntity(onPizzaIngredients.GetEntityArray()[i]);
                //}

                // Create new PizzaOrder & Pizza.

            }

        }
    }
}