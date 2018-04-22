/*

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IngredientDestroySystem : ComponentSystem
{
    public struct IngridientData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Ingredient> ingridients;
    }

    [Inject] private IngridientData ongridientData;

    protected override void OnUpdate()
    {
        for (int i = 0; i < ongridientData.Length; i++)
        {
            Ingredient arrow = ongridientData.ingridients[i];
            arrow.TimeToLive -= Time.deltaTime;

            if (arrow.TimeToLive <= 0)
            {
                PostUpdateCommands.DestroyEntity(ongridientData.Entities[i]);
            }
            ongridientData.ingridients[i] = arrow;
        }
    }
}

*/