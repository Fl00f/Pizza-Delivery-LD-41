using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class PizzaSpawnSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public SharedComponentDataArray<PizzaSpawnData> SpawnData;
    }

    [Inject] private Data data;

    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; ++i)
        {
            Entity spawnedEntity = data.Entities[i];
            PizzaSpawnData spawnData = data.SpawnData[i];

            PostUpdateCommands.RemoveComponent<PizzaSpawnData>(spawnedEntity);

            PostUpdateCommands.AddSharedComponent(spawnedEntity, new PizzaGroup {PizzaId = spawnData.PizzaId});
            PostUpdateCommands.AddComponent(spawnedEntity, spawnData.Position );
            PostUpdateCommands.AddSharedComponent(spawnedEntity, new IngredientList { Value = spawnData.IngredientList });

            PostUpdateCommands.AddComponent(spawnedEntity, new Pizza {});
            PostUpdateCommands.AddComponent(spawnedEntity, new Heading2D { Value = new float2(0, -1) });
            PostUpdateCommands.AddComponent(spawnedEntity, default(TransformMatrix) );
            PostUpdateCommands.AddSharedComponent(spawnedEntity, BootStrap.PizzaLook);
        
            PostUpdateCommands.AddComponent(spawnedEntity, getPizzaCost(spawnData));
        }
    }

    private PizzaCost getPizzaCost(PizzaSpawnData spawnData) {
        return new PizzaCost { OrderCost = spawnData.IngredientList.Count * 10 };
    }
}