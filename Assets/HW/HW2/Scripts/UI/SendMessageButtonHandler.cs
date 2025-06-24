using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HW2
{
    public class SendMessageButtonHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField messageTextField;
        [SerializeField] private ChatManager chatManager;

        public void OnButtonClicked()
        {
            chatManager.SendMessage(messageTextField.text);
        }
    }
}