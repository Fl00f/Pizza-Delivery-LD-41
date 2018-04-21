using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

[UpdateAfter(typeof(MouseDataSystem))]
public class PlayerMovementSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public ComponentDataArray<Position2D> Position;
        public ComponentDataArray<Heading2D> Heading;
        public ComponentDataArray<PlayerInput> Input;
        public ComponentDataArray<PlayerMoveSpeed> speed;
    }

    [Inject] private Data m_Data;
    [Inject] private MouseData mouseData;

    protected override void OnUpdate()
    {
        if (mouseData.Length == 0)
        {
            return;
        }
        float dt = Time.deltaTime;
        for (int index = 0; index < m_Data.Length; ++index)
        {
            var position = m_Data.Position[index].Value;
            var heading = m_Data.Heading[index].Value;

            var playerInput = m_Data.Input[index];
            //This could be redundant as could just use the setting
            //but maybe we want to do something special with the speed
            //at a later date in other systems
            PlayerMoveSpeed playerMoveSpeed = m_Data.speed[index];
            playerMoveSpeed.speed = BootStrap.GameSettings.PlayerMovementSpeed;
            m_Data.speed[index] = playerMoveSpeed;

            position += dt * playerInput.Move * m_Data.speed[index].speed;

            heading = mouseData.worldPosition[0].Value - position;

            m_Data.Position[index] = new Position2D { Value = position };
            m_Data.Heading[index] = new Heading2D { Value = heading };
        }
    }
}