using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class OnPizzaSystem : ComponentSystem
{
    private struct PizzaData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Pizza> Pizza;
        [ReadOnly] public SharedComponentDataArray<PizzaGroup> PizzaGroup;
        public ComponentDataArray<PizzaCost> PizzaCost;
        [ReadOnly] public SharedComponentDataArray<IngredientList> PizzaOrder;
        public ComponentDataArray<Position2D> Position;
    };

    [Inject] PizzaData pizzaData;
    ComponentGroup IngredientData;

    protected override void OnCreateManager(int capacity)
    {
        IngredientData = GetComponentGroup(typeof(Ingredient), typeof(PizzaGroup));
    }

    protected override void OnUpdate()
    {
        for (var pizzaIndex = 0; pizzaIndex < pizzaData.Length; pizzaIndex++)
        {
            updatePizzaCost(pizzaIndex);

            /*
            Entity pizzaEntity = pizzaData.Entities[pizzaIndex];
            PizzaGroup pizzaGroup = pizzaData.PizzaGroup[pizzaIndex];
            Position2D pizzaPosition = pizzaData.Position[pizzaIndex];
            PizzaCost pizzaCost = pizzaData.PizzaCost[pizzaIndex];
            Debug.Log("PIZZA STATE - " + pizzaGroup.PizzaId + ": " + String.Join("; ", getIngredientsOnPizza(pizzaIndex)));
            */

            if (isPizzaComplete(pizzaIndex)) {
                handlePizzaComplete(pizzaIndex);
            }

        }

        // Debug.Log("***");
    }

    private void handlePizzaComplete (int pizzaIndex) {
        Entity pizzaEntity = pizzaData.Entities[pizzaIndex];
        PizzaGroup pizzaGroup = pizzaData.PizzaGroup[pizzaIndex];
        Position2D pizzaPosition = pizzaData.Position[pizzaIndex];
        PizzaCost pizzaCost = pizzaData.PizzaCost[pizzaIndex];

        // Pizza is done.
        Debug.Log("PIZZA DONE - " + pizzaGroup.PizzaId + ": " + pizzaCost.OrderCost+ " | " + pizzaCost.ActualCost);

        // Update global score.

        // Delete this Pizza.
        MoveSpeed toWindow = new MoveSpeed { speed = BootStrap.GameSettings.ArrowSpeed };
        TimedLife timedLife = new TimedLife { TimeToLive = 10.0f };

        PostUpdateCommands.AddComponent(pizzaEntity, toWindow);
        PostUpdateCommands.AddComponent(pizzaEntity, timedLife);
        PostUpdateCommands.RemoveComponent<PizzaGroup>(pizzaEntity);

        // TODO: While ingredients are flying off with the pizza, arrows could hit them.
        for (int i = 0; i < IngredientData.CalculateLength(); i++) {
            Entity ingredientEntity = IngredientData.GetEntityArray()[i];
            //PostUpdateCommands.AddComponent(ingredientEntity, toWindow);
            //PostUpdateCommands.AddComponent(ingredientEntity, timedLife);
            //PostUpdateCommands.RemoveComponent<PizzaGroup>(ingredientEntity);
            PostUpdateCommands.DestroyEntity(ingredientEntity);
        }

        // Create new Pizza.
        PostUpdateCommands.CreateEntity();
        List<int> pizzaIngredients = new List<int>();
        pizzaIngredients.Add(1);

        PostUpdateCommands.AddSharedComponent(new PizzaSpawnData
        {
            Position = pizzaPosition,
            IngredientList = pizzaIngredients
        });

    }

    private bool isPizzaComplete (int pizzaIndex) {
        List<int> currentIngredients = getIngredientsOnPizza(pizzaIndex);
        IngredientList ingredientList = pizzaData.PizzaOrder[pizzaIndex];

        List<int> missingIngredients = ingredientList.Value.Except<int>(currentIngredients).ToList();

        return missingIngredients.Count == 0;
    }

    private void updatePizzaCost (int pizzaIndex) {
        List<int> currentIngredients = getIngredientsOnPizza(pizzaIndex);

        PizzaCost pizzaCost = pizzaData.PizzaCost[pizzaIndex];
        IngredientList ingredientList = pizzaData.PizzaOrder[pizzaIndex];

        List<int> missingIngredients = ingredientList.Value.Except<int>(currentIngredients).ToList();
        List<int> extraIngredients = currentIngredients.Except<int>(ingredientList.Value).ToList();

        var actualCost = pizzaCost.OrderCost - missingIngredients.Count * 5 - extraIngredients.Count * 5;

        pizzaCost.ActualCost = actualCost;
        pizzaData.PizzaCost[pizzaIndex] = pizzaCost;
    }

    private List<int> getIngredientsOnPizza (int pizzaIndex) {
        List<int> currentIngredients = new List<int>();
        IngredientData.SetFilter(pizzaData.PizzaGroup[pizzaIndex]);

        var length = IngredientData.CalculateLength();
        for (int i = 0; i < length; i++)
        {
            currentIngredients.Add(IngredientData.GetComponentDataArray<Ingredient>()[i].IngredientType);
        }

        return currentIngredients;
    }
}