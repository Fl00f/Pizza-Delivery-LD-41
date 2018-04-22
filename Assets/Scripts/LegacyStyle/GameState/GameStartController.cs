using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartController : MonoBehaviour
{
    public void StartNewGame()
    {
        BootStrap.NewGame();
    }

    public void GameOver()
    {
        BootStrap.GameOver();
    }

    public void ResartSimulation()
    {
        BootStrap.RestartGame();
    }
}