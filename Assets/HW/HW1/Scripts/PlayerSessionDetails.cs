using Fusion;
using TMPro;
using UnityEngine;
namespace HW1
{
    public class PlayerSessionDetails : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;

        public void InitializeData(PlayerRef player)
        {
            playerName.text = player.PlayerId.ToString();
        }
    }
}

