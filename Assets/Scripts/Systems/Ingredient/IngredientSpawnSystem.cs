using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class IngredientSpawnSystem : ComponentSystem
{
    public struct IngridientSpawns
    {
        public int Length;
        public EntityArray SpawnedEntities;
        public ComponentDataArray<IngridientSpawnData> SpawnData;
    }

    [Inject] private IngridientSpawns spawnStuffs;

    protected override void OnUpdate()
    {
        var em = PostUpdateCommands;

        for (int i = 0; i < spawnStuffs.Length; ++i)
        {
            var sd = spawnStuffs.SpawnData[i];
            var ingridientEntity = spawnStuffs.SpawnedEntities[i];

            em.RemoveComponent<IngridientSpawnData>(ingridientEntity);
            em.AddComponent(ingridientEntity, new Ingredient() {});
            em.AddComponent(ingridientEntity, new TimedLife() { TimeToLive = BootStrap.GameSettings.FlyingIngredientLifeTime });
            em.AddComponent(ingridientEntity, new MoveSpeed() { speed = BootStrap.GameSettings.FlyingIngredientSpeed });
            em.AddComponent(ingridientEntity, sd.spawnPosition);
            em.AddComponent(ingridientEntity, sd.spawnHeading);

            em.AddComponent(ingridientEntity, default(TransformMatrix));

            em.AddSharedComponent(ingridientEntity, BootStrap.IngredientDefaultLook);
        }
    }
}