using Fusion;
using TMPro;
using UnityEngine;

public class PlayerSessionDetails : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;

    public void InitializeData(PlayerRef player)
    {
        playerName.text = player.PlayerId.ToString();
    }
}
