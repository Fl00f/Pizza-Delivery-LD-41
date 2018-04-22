using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class ArrowInterceptSystem : ComponentSystem
{
    private struct ArrowData
    {
        public int Length;
        public EntityArray entities;

        public ComponentDataArray<Arrow> arrows;
        public ComponentDataArray<Position2D> arrowPositions;
        public ComponentDataArray<Heading2D> arrowHeadings;
    }

    private struct IngridientData
    {
        public int Length;
        public ComponentDataArray<Ingredient> ingredients;
        public ComponentDataArray<MoveSpeed> moveSpeeds; // Used as a filter. Refactor.
        public ComponentDataArray<Position2D> ingredientPositions;
        public ComponentDataArray<Heading2D> ingredientHeadings;
    }

    [Inject] private ArrowData arrowData;
    [Inject] private IngridientData ingredientData;

    private float distanceToCatchIngridient = .5f;

    protected override void OnUpdate()
    {
        for (int index = 0; index < arrowData.Length; index++)
        {
            for (int inIndex = 0; inIndex < ingredientData.Length; inIndex++)
            {
                Ingredient ingredient = ingredientData.ingredients[inIndex];

                float2 delta = arrowData.arrowPositions[index].Value - ingredientData.ingredientPositions[inIndex].Value;
                float distance = math.sqrt(math.pow(delta.x, 2) + math.pow(delta.y, 2));

                if (distance < distanceToCatchIngridient)
                {
                    //ingredient.TimeToLive = 5;

                    ingredientData.ingredients[inIndex] = ingredient;

                    Heading2D arrowHeading = arrowData.arrowHeadings[index];
                    Position2D arrowPosition = arrowData.arrowPositions[index];

                    ingredientData.ingredientHeadings[inIndex] = arrowHeading;
                    ingredientData.ingredientPositions[inIndex] = arrowPosition;
                }
            }
        }
    }
}