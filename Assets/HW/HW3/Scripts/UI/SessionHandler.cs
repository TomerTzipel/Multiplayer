using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HW3
{
    public class SessionHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text sessionName;
        [SerializeField] private TMP_Text playersCount;
        [SerializeField] private Button joinButton;

        private string _sessionName;
        public void InitialzieData(SessionInfo sessionInfo)
        {
            _sessionName = sessionInfo.Name;
            sessionName.text = sessionInfo.Name;
            playersCount.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
            
            joinButton.interactable = true;
        }

        public void JoinSession()
        {
            NetworkManager.Instance.JoinSession(_sessionName);
        }

        public void LockSession()
        {
                joinButton.interactable = false;
                sessionName.text += " (Locked)";
        }
    }

}
