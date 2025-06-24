using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HW2
{
    public class ChatManager : NetworkBehaviour
    {
        [SerializeField] private GameObject chatPanel;
        [SerializeField] private Transform chatScrollViewContent;
        [SerializeField] private GameObject messagePrefab;
        
        private int messageCount = 0;

        public override void Spawned()
        {
            chatPanel.SetActive(false);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SendMessage_RPC()
        {
            
        }

        public void ShowMessage(string messageText)
        {
            if (!chatPanel.activeSelf)
                chatPanel.SetActive(true);
            
            GameObject chatMessage = Instantiate(messagePrefab, Vector3.one, Quaternion.identity, chatScrollViewContent);
            chatMessage.GetComponent<Message>().message.text = messageText;
            
            messageCount++;
            StartCoroutine(DisableChat());
        }

        private IEnumerator DisableChat()
        {
            int currentMessageCount = messageCount;
            Debug.Log(currentMessageCount);
            yield return new WaitForSeconds(7);
            Debug.Log(messageCount);
            if (currentMessageCount == messageCount)
                chatPanel.SetActive(false);
        }
    }
}
