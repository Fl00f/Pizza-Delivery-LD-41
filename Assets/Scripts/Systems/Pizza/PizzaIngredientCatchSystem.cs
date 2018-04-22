using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class PizzaIngredientCatchSystem : ComponentSystem
{
    private struct PizzaData
    {
        public int Length;
        public ComponentDataArray<Pizza> pizzas;
        [ReadOnly] public SharedComponentDataArray<PizzaGroup> pizzaGroups;
        public ComponentDataArray<Position2D> pizzaPositions;
    }

    private struct IngredientData
    {
        public int Length;
        public EntityArray entities;
        public ComponentDataArray<Ingredient> ingredients;
        public ComponentDataArray<MoveSpeed> moveSpeed;
        public ComponentDataArray<Position2D> ingredientPositions;
        // public ComponentDataArray<Heading2D> ingredientHeadings;
    }

    [Inject] private PizzaData pizzaData;
    [Inject] private IngredientData ingredientData;

    private float distanceToCatchIngridient = .5f;

    protected override void OnUpdate()
    {
        for (int pizzaIndex = 0; pizzaIndex < pizzaData.Length; pizzaIndex++)
        {
            for (int ingredientIndex = 0; ingredientIndex < ingredientData.Length; ingredientIndex++)
            {
                // Ingredient ingredient = ingridientData.ingredients[inIndex];

                float2 delta = pizzaData.pizzaPositions[pizzaIndex].Value - ingredientData.ingredientPositions[ingredientIndex].Value;
                float distance = math.sqrt(math.pow(delta.x, 2) + math.pow(delta.y, 2));

                if (distance < distanceToCatchIngridient)
                {
                    handleIngredientHitPizza(ingredientIndex, pizzaIndex);

                    /*
                    PostUpdateCommands.CreateEntity(BootStrap.IngredientPizzaSpawnArchetype);

                    PostUpdateCommands.SetComponent(new IngredientSpawnOnPizzaData()
                    {
                        heading = ingridientData.ingredientHeadings[inIndex].Value,
                        position = ingridientData.ingredientPositions[inIndex].Value,
                        // OnPizza = pizzaData.pizzas[index]
                    });

                    ingridientData.ingredients[inIndex] = ingredient;
                    */
                }
            }
        }
    }

    private void handleIngredientHitPizza (int ingredientIndex, int pizzaIndex) {

        // Note: This component acts as state for if it's on the pizza.
        // Refactor.
        PostUpdateCommands.RemoveComponent<MoveSpeed>(
            ingredientData.entities[ingredientIndex]);

        PostUpdateCommands.RemoveComponent<TimedLife>(
            ingredientData.entities[ingredientIndex]);

        PostUpdateCommands.AddSharedComponent<PizzaGroup>(
            ingredientData.entities[ingredientIndex],
            pizzaData.pizzaGroups[pizzaIndex]);
    }
}