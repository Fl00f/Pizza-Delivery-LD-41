using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TestScoringSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    PostUpdateCommands.CreateEntity();
        //    PostUpdateCommands.AddComponent(new DeductScore { Value = 10 });
        //    PostUpdateCommands.AddSharedComponent(new ScoringGroup { GroupId = 0 });

        //    PostUpdateCommands.CreateEntity();
        //    PostUpdateCommands.AddComponent(new AddScore { Value = 100 });
        //    PostUpdateCommands.AddSharedComponent(new ScoringGroup { GroupId = 0 });
        //}
    }
}