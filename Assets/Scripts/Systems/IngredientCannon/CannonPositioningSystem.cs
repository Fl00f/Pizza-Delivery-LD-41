using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

public class CannonPositioningSystem : ComponentSystem
{
    private struct CannonsData
    {
        public int Length;
        public ComponentDataArray<IngridientCannon> cannons;
        public ComponentDataArray<Position2D> Positions;
        public ComponentDataArray<Heading2D> Headings;
    }

    [Inject] private CannonsData cannonsData;

    protected override void OnUpdate()
    {
        for (int index = 0; index < cannonsData.Length; index++)
        {
            if (index >= BootStrap.GameSettings.CannonPositions.Count)
            {
                return;
            }

            Position2D position2D = cannonsData.Positions[index];
            Heading2D heading2D = cannonsData.Headings[index];

            Vector2 settingsCannonPos = BootStrap.GameSettings.CannonPositions[index];
            Vector2 settingsCannonHeading = BootStrap.GameSettings.CannonHeadings[index];

            float2 pos = new float2(settingsCannonPos.x, settingsCannonPos.y);
            float2 heading = new float2(settingsCannonHeading.x, settingsCannonHeading.y);

            position2D.Value = pos;
            heading2D.Value = math.normalize(heading);

            cannonsData.Positions[index] = position2D;
            cannonsData.Headings[index] = heading2D;
        }
    }
}