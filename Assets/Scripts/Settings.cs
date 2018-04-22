using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region Arrows

    [Header("Arrows")]
    public float ArrowLifeTime;

    public float ArrowSpeed;

    #endregion Arrows

    #region Ingredient

    [Header("Ingredients")]
    public float FlyingIngredientLifeTime;

    public float FlyingIngredientSpeed;

    #endregion Ingredient

    #region Player

    [Header("Player")]
    public float PlayerMovementSpeed;

    public float PlayerFireRate; //pre second

    public Vector2 PlayerSpawnPosition;

    #endregion Player

    #region Cannon

    [Header("Cannons")]
    public float CannonFireRate;

    public List<Vector2> CannonPositions;
    public List<Vector2> CannonHeadings;
    [HideInInspector] public int CannonsOnByDefault => CannonsOn ? 1 : 0;
    [SerializeField] private bool CannonsOn;

    #endregion Cannon

    private void Start()
    {
        if (CannonPositions.Count != 2 || CannonHeadings.Count != 2)
        {
            throw new System.Exception("Must have at least 2 headings and positions");
        }
    }
}