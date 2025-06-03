using Fusion;
using TMPro;
using UnityEngine;

public class PlayerSessionDetails : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    
    private PlayerRef player;

    public void InitializeData(PlayerRef player)
    {
        this.player = player;
        playerName.text = player.PlayerId.ToString();
    }
}
