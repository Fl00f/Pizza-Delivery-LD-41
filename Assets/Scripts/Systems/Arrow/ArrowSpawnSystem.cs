using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class ShotSpawnSystem : ComponentSystem
{
    public struct SpawnStuffs
    {
        public int Length;
        public EntityArray SpawnedEntities;
        [ReadOnly] public ComponentDataArray<ArrowSpawnData> SpawnData;
    }

    [Inject] private SpawnStuffs spawnStuffs;
    [Inject] private MouseData mouseData;

    protected override void OnUpdate()
    {
        var em = PostUpdateCommands;

        for (int i = 0; i < spawnStuffs.Length; ++i)
        {
            var sd = spawnStuffs.SpawnData[i];
            var arrowEntity = spawnStuffs.SpawnedEntities[i];

            em.RemoveComponent<ArrowSpawnData>(arrowEntity);
            em.AddComponent(arrowEntity, new Arrow() { TimeToLive = 5 });
            em.AddComponent(arrowEntity, new MoveSpeed() { speed = 1 });
            em.AddComponent(arrowEntity, sd.Position);

            //Cant just use sd.Heading for some reason. Look into
            var position = sd.Position.Value;
            var heading = sd.Heading.Value;

            heading = mouseData.worldPosition[0].Value - position;

            em.AddComponent(arrowEntity, new Heading2D { Value = heading });
            //em.AddComponent(arrowEntity, sd.Heading);

            em.AddComponent(arrowEntity, default(TransformMatrix));

            em.AddSharedComponent(arrowEntity, BootStrap.ArrowLook);
        }
    }
}