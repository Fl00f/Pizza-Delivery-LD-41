using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

public class PlayerFireSystem : ComponentSystem
{
    private struct PlayerInputData
    {
        public int Length;
        public ComponentDataArray<PlayerInput> Input;
        public ComponentDataArray<Position2D> Position;
        public ComponentDataArray<Heading2D> Heading;
    }

    [Inject] private PlayerInputData playerInputData;

    protected override void OnUpdate()
    {
        for (int index = 0; index < playerInputData.Length; index++)
        {
            PlayerInput playerInput = playerInputData.Input[index];
            var position = playerInputData.Position[index].Value;
            var heading = playerInputData.Heading[index].Value;

            //TODO: Move Input.GetMouseButtonDown to PlayerInputSystem
            if (playerInput.CoolDownFinished && Input.GetMouseButtonDown(0))
            {
                heading = math.normalize(playerInput.PlayerHeading);

                playerInput.FireCooldown = .2f;

                PostUpdateCommands.CreateEntity(BootStrap.ArrowArchetype);

                PostUpdateCommands.SetComponent(new ArrowSpawnData
                {
                    Position = new Position2D { Value = position },
                    Heading = new Heading2D { Value = heading }
                });
            }

            playerInputData.Input[index] = playerInput;
        }
    }
}