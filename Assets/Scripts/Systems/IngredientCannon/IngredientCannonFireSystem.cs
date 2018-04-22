using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

public class IngredientCannonFireSystem : ComponentSystem
{
    private struct CannonData
    {
        public int Length;
        public ComponentDataArray<IngridientCannon> cannons;
        public ComponentDataArray<Heading2D> headings;
        public ComponentDataArray<Position2D> positions;
    }

    [Inject] private CannonData cannonData;

    protected override void OnUpdate()
    {
        for (int i = 0; i < cannonData.Length; i++)
        {
            IngridientCannon cannon = cannonData.cannons[i];
            cannon.FireCooldown -= Time.deltaTime;

            if (cannon.CanFire)
            {
                //Fire
                var position = cannonData.positions[i].Value;
                var heading = cannonData.headings[i].Value;

                PostUpdateCommands.CreateEntity(BootStrap.IngredientSpawnArchetype);

                PostUpdateCommands.SetComponent(new IngridientSpawnData
                {
                    spawnPosition = new Position2D { Value = position },
                    spawnHeading = new Heading2D { Value = heading },
                    IngridientType = Random.Range(0, BootStrap.IngredientLooks.Count) //Random for now
                });

                cannon.FireCooldown = BootStrap.GameSettings.CannonFireRate;
            }

            cannonData.cannons[i] = cannon;
        }
    }
}