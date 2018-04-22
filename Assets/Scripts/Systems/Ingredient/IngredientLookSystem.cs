using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class IngredientLookSystem : ComponentSystem
{
    private struct IngredientsJustSpawnedData
    {
        public int Length;
        public ComponentDataArray<Ingredient> ingredients;
        public ComponentDataArray<IngredientJustSpawn> justSpawnTag;
        public EntityArray entities;
    }

    [Inject] private IngredientsJustSpawnedData ingredientData;

    protected override void OnUpdate()
    {
        for (int i = 0; i < ingredientData.Length; i++)
        {
            PostUpdateCommands.RemoveComponent<IngredientJustSpawn>(ingredientData.entities[i]);
            PostUpdateCommands.AddSharedComponent(ingredientData.entities[i], GetIngredientLook(ingredientData.ingredients[i].IngredientType));
            //PostUpdateCommands.AddSharedComponent(ingredientData.entities[i], BootStrap.IngredientDefaultLook);
        }
    }

    private MeshInstanceRenderer GetIngredientLook(int ingredientType)
    {
        return BootStrap.GetIngredientLook(ingredientType);
    }
}