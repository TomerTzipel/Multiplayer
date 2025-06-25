using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;

namespace HW2
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
        
        [SerializeField] private InputActionAsset inputActions;
        private InputAction _openChatAction;
        
        private bool _areChatInteractablesEnabled;
        private int _messageCount = 0;

        private void OnEnable()
        {
            inputActions.FindActionMap("UI").Enable();
        }

        private void OnDisable()
        {
            inputActions.FindActionMap("UI").Disable();
        }

        public void Awake()
        {
            EnableChatInteractables(false);
            chatPanel.SetActive(false);
            _openChatAction = inputActions.FindAction("OpenChat");
        }

        private void Update()
        {
            EnableChat();
        }
        
        public void ShowMessage(string messageText)
        {
            if (!chatPanel.activeSelf)
                chatPanel.SetActive(true);
            
            Message chatMessage = Instantiate(messagePrefab, Vector3.one, Quaternion.identity, chatScrollViewContent);
            chatMessage.message.text = messageText;
            
            _messageCount++;
            StartCoroutine(DisableChat());
        }
        
        public void ShowMessage(string messageText, UserData userData)
        {
            if (!chatPanel.activeSelf)
                chatPanel.SetActive(true);
            
            Message chatMessage = Instantiate(messagePrefab, Vector3.one, Quaternion.identity, chatScrollViewContent);
            chatMessage.message.text = $"{userData.nickname}: {messageText}";
            chatMessage.message.color = userData.color;
            
            _messageCount++;
            StartCoroutine(DisableChat());
        }

        public void OnSendButtonClicked()
        {
            string messageText = messageInputField.text;
            if (messageText.StartsWith("/w"))
            {
                string targetName = messageInputField.text.Split(' ')[1];
                string noArgumentsMessage = messageInputField.text.Split($"/w {targetName} ")[1];
                Debug.Log($"{targetName}(whisper): {noArgumentsMessage}");
                chatNetworkManager.SendWhisper_RPC(noArgumentsMessage, targetName);
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
            _areChatInteractablesEnabled = value;
            messageInputField.interactable = value;
            sendMessageButton.interactable = value;
        }

        private void EnableChat()
        {
            if (!chatPanel.activeSelf && _openChatAction.WasPressedThisFrame())
            {
                chatPanel.SetActive(true);
                if (!_areChatInteractablesEnabled) StartCoroutine(DisableChat());
            }
        }

        private IEnumerator DisableChat()
        {
            int currentMessageCount = _messageCount;
            yield return new WaitForSeconds(5);
            if (currentMessageCount == _messageCount)
                chatPanel.SetActive(false);
        }
    }
}
