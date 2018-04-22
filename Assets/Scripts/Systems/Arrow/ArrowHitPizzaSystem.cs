using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

public class ArrowHitPizzaSystem : ComponentSystem
{
    private struct PizzaData
    {
        public int Length;
        public ComponentDataArray<Pizza> pizzas;
        public ComponentDataArray<Position2D> pizzaPositions;
    }

    private struct ArrowData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Arrow> arrows;
        public ComponentDataArray<Position2D> arrowPositions;
    }

    [Inject] private PizzaData pizzaData;
    [Inject] private ArrowData arrowData;

    private float distanceToCatchArrow = .5f;

    protected override void OnUpdate()
    {
        for (int arrowIndex = 0; arrowIndex < arrowData.Length; arrowIndex++)
        {
            for (int pizzaIndex = 0; pizzaIndex < pizzaData.Length; pizzaIndex++)
            {                     
                float2 delta = arrowData.arrowPositions[arrowIndex].Value - pizzaData.pizzaPositions[pizzaIndex].Value;
                float distance = math.sqrt(math.pow(delta.x, 2) + math.pow(delta.y, 2));

                if (distance < distanceToCatchArrow)
                {
                    PostUpdateCommands.DestroyEntity(arrowData.Entities[arrowIndex]);
                }
            }
        }
    }
}