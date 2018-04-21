using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ArrowDestroySystem : ComponentSystem
{
    public struct ArrowData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Arrow> arrows;
    }

    [Inject] private ArrowData arrowData;

    protected override void OnUpdate()
    {
        for (int i = 0; i < arrowData.Length; i++)
        {
            Arrow arrow = arrowData.arrows[i];
            arrow.TimeToLive -= Time.deltaTime;

            if (arrow.TimeToLive <= 0)
            {
                PostUpdateCommands.DestroyEntity(arrowData.Entities[i]);
            }
            arrowData.arrows[i] = arrow;
        }
    }
}