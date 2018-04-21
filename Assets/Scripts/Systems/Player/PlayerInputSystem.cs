using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    private struct PlayerData
    {
        public int Length;

        public ComponentDataArray<PlayerInput> Input;
    }

    [Inject] private PlayerData m_Players;

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < m_Players.Length; ++i)
        {
            UpdatePlayerInput(i, dt);
        }
    }

    private void UpdatePlayerInput(int i, float dt)
    {
        //TODO: Get mouse input

        PlayerInput pi = m_Players.Input[i];

        pi.Move.x = Input.GetAxis("Horizontal");
        pi.Move.y = Input.GetAxis("Vertical");
        pi.PlayerHeading.x = Input.mousePosition.x;
        pi.PlayerHeading.y = Input.mousePosition.y;

        if (!pi.CoolDownFinished)
        {
            pi.FireCooldown = Mathf.Max(0.0f, m_Players.Input[i].FireCooldown - dt);
        }

        m_Players.Input[i] = pi;
    }
}