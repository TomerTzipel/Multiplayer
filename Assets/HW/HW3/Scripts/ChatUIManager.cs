
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using WebSocketSharp;
using HW2;

namespace HW3
{
    public class ChatUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject chatPanel;
        [SerializeField] private Transform chatScrollViewContent;
        [SerializeField] private Message messagePrefab;
        [SerializeField] private CharacterSelectionManager characterSelectionManager;
        [SerializeField] private GameObject namePanel;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private ColorPickerHandler colorPicker;
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private ChatNetworkManager chatNetworkManager;
        [SerializeField] private Button userDataConfirmationButton;
        [SerializeField] private Button sendMessageButton;

        [SerializeField] private int messageDuration;

        private InputSystem_Actions inputSystemActions;

        private int _messageCount = 0;
        private string _selfName;
        private bool _chatOpenedByUser = false;

        public void Awake()
        {
            inputSystemActions = new InputSystem_Actions();
            EnableChatInteractables(false);
            chatPanel.SetActive(false);
        }

        private void OnEnable()
        {
           
            inputSystemActions.UI.Enable();
            inputSystemActions.UI.OpenChat.started += EnableChat;
        }

        private void OnDisable()
        {
            inputSystemActions.UI.OpenChat.started -= EnableChat;
            inputSystemActions.UI.Disable();
        }
  
        public void ShowMessage(string messageText)
        {
            if (!chatPanel.activeSelf)
                chatPanel.SetActive(true);
            
            Message chatMessage = Instantiate(messagePrefab, Vector3.one, Quaternion.identity, chatScrollViewContent);
            chatMessage.message.text = messageText;
            
            StartCoroutine(DisableChat());
        }
        
        public void ShowMessage(string messageText, UserData userData)
        {
            if (!chatPanel.activeSelf)
                chatPanel.SetActive(true);
            
            Message chatMessage = Instantiate(messagePrefab, Vector3.one, Quaternion.identity, chatScrollViewContent);
            chatMessage.message.text = $"{userData.nickname}: {messageText}";
            chatMessage.message.color = userData.color;
   
            StartCoroutine(DisableChat());
        }

        public void OnSendButtonClicked()
        {
            string messageText = messageInputField.text;
            if (messageText.StartsWith("/w"))
            {
                string targetName = messageInputField.text.Split(' ')[1];

                if (targetName == _selfName) return;

                string noArgumentsMessage = messageInputField.text.Split($"/w {targetName} ")[1];
                string message = $"({targetName}) {noArgumentsMessage}";
                chatNetworkManager.SendWhisper_RPC(message, targetName);
            }
            else
            {
                chatNetworkManager.SendMessage_RPC(messageText);
            }
            messageInputField.text = string.Empty;
        }

        public void OnNicknameButtonClicked()
        {
            string playerName = nameInputField.text;
            Color playerColor = colorPicker.CurrentColor;
            if(playerName.IsNullOrEmpty())
            {
                ShowMessage("Name can't be empty");
                return;
            }
            if (!playerName.All(char.IsLetterOrDigit))
            {
                ShowMessage("Name must be letters and letters");
                return;
            }

            _selfName = playerName;
            EnableUserDataConfirmationButton(false);
            characterSelectionManager.InitializeUserData(new UserData{nickname=playerName, color=playerColor});
            characterSelectionManager.ConfirmPlayerData_RPC(playerName, playerColor);
        }

        public void EnableUserDataConfirmationButton(bool value)
        {
            userDataConfirmationButton.interactable = value;
        }

        public void EnableChatInteractables(bool value)
        {
            messageInputField.interactable = value;
            sendMessageButton.interactable = value;
        }

        private void EnableChat(InputAction.CallbackContext context)
        {
            if (!chatPanel.activeSelf)
            {
                chatPanel.SetActive(true);
                _chatOpenedByUser = true;
            }
            else
            {
                chatPanel.SetActive(false);
                _chatOpenedByUser = false;
            }
        }

        private IEnumerator DisableChat()
        {
            _messageCount++;
            int currentMessageCount = _messageCount;
            yield return new WaitForSeconds(messageDuration);
            if (currentMessageCount == _messageCount && !_chatOpenedByUser)
            {
                chatPanel.SetActive(false);
            }       
        }
    }
}
