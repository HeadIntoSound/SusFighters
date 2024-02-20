using UnityEngine;
using TMPro;
using System;

// Controls the changing elements on the UI
public class UIController : MonoBehaviour
{
    [SerializeField] GameObject[] panel;        // The panel with the portrait, health and points of each player
    [SerializeField] TMP_Text[] healthLabel;
    [SerializeField] TMP_Text[] pointsLabel;

    private void Start()
    {
        EventManager.Instance.OnHealthChange.AddListener(UpdateHealth);
        EventManager.Instance.OnPointsChange.AddListener(UpdatePoints);
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnHealthChange.RemoveListener(UpdateHealth);
    }

    private void UpdateHealth(float value, int playerSlot)
    {
        panel[playerSlot].SetActive(true);
        healthLabel[playerSlot].text = String.Format("{0}%", value);
    }
    private void UpdatePoints(int points, int playerSlot)
    {
        panel[playerSlot].SetActive(true);
        pointsLabel[playerSlot].text = points.ToString();
    }
}
