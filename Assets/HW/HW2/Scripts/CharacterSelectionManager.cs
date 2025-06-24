using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HW2
{
    public class CharacterSelectionManager : NetworkBehaviour
    {
        private const string LOBBY_SCENE_NAME = "LobbyScene2";

        [SerializeField] private PlayableCharacterController[] characterPrefabs;
        [SerializeField] private Transform[] characterSpawnPositions;
        [SerializeField] private CharacterButtonHandler[] buttonHandlers;
        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject finishGameButton;
        [SerializeField] private GameObject namePanel;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private ChatManager chatManager;
        [SerializeField] private GameObject chatPanel;

        private bool[] charactersPickStatus;
        private string playerName;

        public override void Spawned()
        { 
            if (characterPrefabs.Length != characterSpawnPositions.Length || characterPrefabs.Length != buttonHandlers.Length) Debug.LogError("Characters Arrays Size Mismatch!");

            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                buttonHandlers[i].Initialize(this,i, characterPrefabs[i].Settings);
            }

            charactersPickStatus = new bool[characterPrefabs.Length];

            namePanel.SetActive(true);
            selectionPanel.SetActive(false);
            finishGameButton.SetActive(false);
            gameOverPanel.SetActive(false);
            chatPanel.SetActive(false);
        }

        public void EnableAllButtons(bool value)
        {
            foreach (var button in buttonHandlers)
            {
                button.EnableButton(value);
            }
        }

        public void ConfirmPlayerName()
        {
            playerName = nameInputField.text;
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
            if (!charactersPickStatus[characterIndex])
            {
                //The character is available
                charactersPickStatus[characterIndex] = true;
                isAvailable = true;
            }

            CharacterRequestResult_RPC(info.Source, isAvailable, characterIndex);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void CharacterRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool isAvailable,int characterIndex)
        {
            if (!isAvailable)
            {
                chatManager.ShowMessage("Game: Character already taken");
                EnableAllButtons(true);
                Debug.Log("ALREADY TAKEN BE FASTER");
                return;
            }

            //Spawn the correct prefab at the correct position
            PlayableCharacterController characterPrefab = characterPrefabs[characterIndex];
            Vector3 position = characterSpawnPositions[characterIndex].position;
            var character = NetworkManager.Instance.NetworkRunner.Spawn(characterPrefab, position,onBeforeSpawned: InitializeCharacter);

            selectionPanel.SetActive(false);
            if (NetworkManager.Instance.NetworkRunner.IsSharedModeMasterClient) finishGameButton.SetActive(true);
        }

        private void InitializeCharacter(NetworkRunner runner, NetworkObject obj)
        {
            //Sadly there is no other way but GetComponent :( (That I found atleast)
            obj.GetComponent<PlayableCharacterController>().Initialize(playerName);
        }

    }
}


