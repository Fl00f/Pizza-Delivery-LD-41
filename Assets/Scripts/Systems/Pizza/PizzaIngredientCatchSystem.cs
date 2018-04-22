using System.Collections;
using System.Collections.Generic;
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
        public ComponentDataArray<Position2D> pizzaPositions;
    }

    private struct IngridientData
    {
        public int Length;
        public EntityArray entities;
        public ComponentDataArray<Ingredient> ingredients;
        public ComponentDataArray<Position2D> ingredientPositions;
        public ComponentDataArray<Heading2D> ingredientHeadings;
    }

    [Inject] private PizzaData pizzaData;
    [Inject] private IngridientData ingridientData;

    private float distanceToCatchIngridient = .5f;

    protected override void OnUpdate()
    {
        for (int index = 0; index < pizzaData.Length; index++)
        {
            for (int inIndex = 0; inIndex < ingridientData.Length; inIndex++)
            {
                Ingredient ingredient = ingridientData.ingredients[inIndex];

                if (ingredient.TimeToLive <= 0) return;

                float2 delta = pizzaData.pizzaPositions[index].Value - ingridientData.ingredientPositions[inIndex].Value;
                float distance = math.sqrt(math.pow(delta.x, 2) + math.pow(delta.y, 2));

                if (distance < distanceToCatchIngridient)
                {
                    PostUpdateCommands.CreateEntity(BootStrap.IngredientPizzaSpawnArchetype);

                    PostUpdateCommands.SetComponent(new IngredientSpawnOnPizzaData()
                    {
                        heading = ingridientData.ingredientHeadings[inIndex].Value,
                        position = ingridientData.ingredientPositions[inIndex].Value,
                        // OnPizza = pizzaData.pizzas[index]
                    });

                    ingredient.TimeToLive = 0;

                    ingridientData.ingredients[inIndex] = ingredient;
                }
            }
        }
    }
}