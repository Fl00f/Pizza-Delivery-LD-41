using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class PizzaIngredientSpawnSystem : ComponentSystem
{
    private struct CaughtIngredientsData
    {
        public int Length;
        public EntityArray entities;
        public ComponentDataArray<IngredientSpawnOnPizzaData> caughtIngredients;
    }

    [Inject] private CaughtIngredientsData caughtIngredientsData;

    protected override void OnUpdate()
    {
        for (int index = 0; index < caughtIngredientsData.Length; index++)
        {
            var position = caughtIngredientsData.caughtIngredients[index].position;
            var heading = caughtIngredientsData.caughtIngredients[index].heading;

            PostUpdateCommands.DestroyEntity(caughtIngredientsData.entities[index]);

            PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(new OnPizzaIngredient() { OnPizza = caughtIngredientsData.caughtIngredients[index].OnPizza });
            PostUpdateCommands.AddComponent(new Heading2D() { Value = heading });
            PostUpdateCommands.AddComponent(new Position2D() { Value = position });
            PostUpdateCommands.AddComponent(default(TransformMatrix));

            PostUpdateCommands.AddSharedComponent(BootStrap.IngredientLook); //TODO: Set by ingredient type
        }
    }
}