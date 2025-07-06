using Fusion;
using UnityEngine;
using HW2;

namespace HW3
{
    public class ChatNetworkManager : NetworkBehaviour
    {
        [SerializeField] private UserDataManager userDataManager;
        [SerializeField] private ChatUIManager chatUIManager;
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SendMessage_RPC(string message, RpcInfo info = default)
        {
            UserData source = userDataManager.UserDataDict[info.Source];
            
            TransmitMessage_RPC(message, source.nickname, source.color);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SendWhisper_RPC(string message, string targetName, RpcInfo info = default)
        {
            UserData source = userDataManager.UserDataDict[info.Source];
            PlayerRef target = PlayerRef.None;
            bool targetFound = false;

            foreach (var player in userDataManager.UserDataDict.Keys)
            {
                if (userDataManager.UserDataDict[player].nickname == targetName)
                {
                    target = player;
                    targetFound = true;
                    break;
                }
            }

            if (targetFound)
            {
                TransmitWhisper_RPC(message, source.nickname, source.color, target);
                TransmitWhisper_RPC(message, source.nickname, source.color, info.Source);
            }
            else 
                chatUIManager.ShowMessage("Game: player name not found");
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void TransmitMessage_RPC(string message, string nickname, Color color)
        {
            chatUIManager.ShowMessage(message, new UserData{nickname = nickname, color = color});
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void TransmitWhisper_RPC(string message, string nickname, Color color, [RpcTarget] PlayerRef target)
        {
            chatUIManager.ShowMessage(message, new UserData{nickname = nickname, color = color});
        }
    }
}
