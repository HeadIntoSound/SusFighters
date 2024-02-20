using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the color of the player, blue for player 1, red for player 2
public class SkinController : MonoBehaviour
{
    [SerializeField] Color[] playerColors;  // An array containing the colors for each player
    [SerializeField] Renderer[] renderers; // Renderers of each model that make the character as a whole

    public void SetColor(int playerIndex)
    {
        foreach (var r in renderers)
        {
            r.material.color = playerColors[playerIndex];
        }
    }

}
