using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using HW2;
using Unity.Cinemachine;
using UnityEngine.UI;

namespace HW3
{
    public class GameSessionManager : NetworkBehaviour
    {
        private const string LOBBY_SCENE_NAME = "LobbyScene3";

        [SerializeField] private CinemachineCamera cineCam;
        [SerializeField] private Camera mainCamera;

        [SerializeField] private PlayerController[] characterPrefabs;
        [SerializeField] private Transform[] characterSpawnPositions;
        [SerializeField] private CharacterButtonHandler[] buttonHandlers;

        [SerializeField] private UserDataManager userDataManager;
        [SerializeField] private ChatNetworkManager chatNetworkManager;
        [SerializeField] private ChatUIManager chatUIManager;

        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject namePanel;
        [SerializeField] private GameObject chatPanel;

       
        [SerializeField] private GameObject finishGameButton;
        [SerializeField] private Toggle sessionLockToggle;

        private bool[] _charactersPickStatus;

        public override void Spawned()
        { 
            if (characterPrefabs.Length != characterSpawnPositions.Length || characterPrefabs.Length != buttonHandlers.Length) Debug.LogError("Characters Arrays Size Mismatch!");

            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                var settings = characterPrefabs[i].Settings;
                buttonHandlers[i].Initialize(this,i, settings.Name, settings.Splash);
            }

            _charactersPickStatus = new bool[characterPrefabs.Length];

            namePanel.SetActive(true);
            selectionPanel.SetActive(false);
            finishGameButton.SetActive(false);
            gameOverPanel.SetActive(false);
            finishGameButton.SetActive(false);
            sessionLockToggle.gameObject.SetActive(false);
        }

        public void EnableAllButtons(bool value)
        {
            foreach (var button in buttonHandlers)
            {
                button.EnableButton(value);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void ConfirmPlayerData_RPC(string playerName, Color playerColor, RpcInfo info = default)
        {
            bool result = userDataManager.TryAddUserData(info.Source, new UserData{nickname=playerName, color=playerColor});        
            ConfirmPlayerSelectionResult_RPC(info.Source, result);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void ConfirmPlayerSelectionResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
        {
            if (!result)
            {
                chatUIManager.ShowMessage("Name or color already in use.");
                chatUIManager.EnableUserDataConfirmationButton(true);
                return;
            }
            
            chatUIManager.EnableChatInteractables(true);
            namePanel.SetActive(false);
            selectionPanel.SetActive(true);
        }

        public async void LeaveGame()
        {
            await NetworkManager.Instance.NetworkRunner.Shutdown();
            SceneManager.LoadScene(LOBBY_SCENE_NAME);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void EndGame_RPC()
        {
            gameOverPanel.SetActive(true);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RequestCharacter_RPC(int characterIndex,RpcInfo info = default)
        {
            bool isAvailable = false;
            if (!_charactersPickStatus[characterIndex])
            {
                //The character is available
                _charactersPickStatus[characterIndex] = true;
                isAvailable = true;
            }

            CharacterRequestResult_RPC(info.Source, isAvailable, characterIndex);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void CharacterRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool isAvailable,int characterIndex)
        {
            if (!isAvailable)
            {
                chatUIManager.ShowMessage("Game: Character already taken");
                EnableAllButtons(true);
                return;
            }

            //Spawn the correct prefab at the correct position
            PlayerController characterPrefab = characterPrefabs[characterIndex];
            Vector3 position = characterSpawnPositions[characterIndex].position;
            var character = NetworkManager.Instance.NetworkRunner.Spawn(characterPrefab, position,onBeforeSpawned: InitializeCharacter);

            selectionPanel.SetActive(false);
            if (NetworkManager.Instance.NetworkRunner.IsSharedModeMasterClient) finishGameButton.SetActive(true);
            if (NetworkManager.Instance.NetworkRunner.IsSharedModeMasterClient) sessionLockToggle.gameObject.SetActive(true);
        }

        private void InitializeCharacter(NetworkRunner runner, NetworkObject obj)
        {
            cineCam.LookAt = obj.transform;
            cineCam.Follow = obj.transform;
            UserData userData = chatUIManager.UserData;
            obj.GetComponent<PlayerController>().NetworkInitialize(userData.nickname, userData.color, mainCamera);
        }

        public void OnLockSessionStateChanged(bool value)
        {
            NetworkManager.Instance.NetworkRunner.SessionInfo.IsOpen = !value;
        }
    }
}


