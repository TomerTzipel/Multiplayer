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
            
            EnableButton();
        }

        public void JoinSession()
        {
            NetworkManager.Instance.JoinSession(_sessionName);
        }

        public void LockSession()
        {
            DisableButton(); 
            sessionName.text += " (Locked)";
        }

        public void DisableButton()
        {
            joinButton.interactable = false; 
        }
        
        public void EnableButton()
        {
            joinButton.interactable = true; 
        }
    }

}
