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
            PostUpdateCommands.AddSharedComponent(ingredientData.entities[i], GetIngredientLook(/*ingredientData.ingredients[i].IngredientType*/ 0));
        }
    }

    private MeshInstanceRenderer GetIngredientLook(int ingredientType)
    {
        switch (ingredientType)
        {
            case 0:
                return BootStrap.IngredientCheeseLook;

            case 1:
                return BootStrap.IngredientChickenLook;

            case 2:
                return BootStrap.IngredientMushroomLook;

            case 3:
                return BootStrap.IngredientOnionLook;

            case 4:
                return BootStrap.IngredientPeperoniLook;

            case 5:
                return BootStrap.IngredientSausageLook;

            default:
                return BootStrap.IngredientDefaultLook;
        }
    }
}