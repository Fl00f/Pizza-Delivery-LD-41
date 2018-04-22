using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameEndSystem : ComponentSystem
{
    private Canvas GameOverScreen;

    protected override void OnUpdate()
    {
        if (GameOverScreen == null)
        {
            return;
        }

        //TODO: Actually check state to see if Screen should show
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameOverScreen.enabled = true;
        }
    }

    public void SetGameOverScreen(Canvas canvas)
    {
        GameOverScreen = canvas;
    }
}