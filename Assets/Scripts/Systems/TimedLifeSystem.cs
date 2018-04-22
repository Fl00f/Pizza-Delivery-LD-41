using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimedLifeSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<TimedLife> timedLives;
    }

    [Inject] private Data data;

    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            TimedLife timedLife = data.timedLives[i];
            timedLife.TimeToLive -= Time.deltaTime;
            data.timedLives[i] = timedLife;

            if (timedLife.TimeToLive <= 0)
            {
                PostUpdateCommands.DestroyEntity(data.Entities[i]);
            }
        }
    }
}