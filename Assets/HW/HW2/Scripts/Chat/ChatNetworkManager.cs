using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HW2
{
    public class ChatNetworkManager : NetworkBehaviour
    {
        [SerializeField] private UserDataManager userDataManager;
        [SerializeField] private ChatUIManager chatUIManager;
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SendMessage_RPC(string message, RpcInfo info = default)
        {
            UserData source = userDataManager.UserDataDict[info.Source];
            TransmitMessage_RPC(message, source);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void TransmitMessage_RPC(string message, UserData sourceUserData)
        {
            chatUIManager.ShowMessage(message, sourceUserData);
        }
    }
}
