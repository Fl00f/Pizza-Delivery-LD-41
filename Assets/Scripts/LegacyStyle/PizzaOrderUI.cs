using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaOrderUI : MonoBehaviour
{
    [SerializeField] private Text IngredientsList;

    [SerializeField] private Text Cost;

    public void SetCostText(string value)
    {
        Cost.text = value;
    }

    public void SetIngredientsListText(string value)
    {
        IngredientsList.text = value;
    }
}