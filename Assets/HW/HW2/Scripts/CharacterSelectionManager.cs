using Fusion;
using UnityEngine;
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

        private bool[] charactersPickStatus;

        public override void Spawned()
        { 
            if (characterPrefabs.Length != characterSpawnPositions.Length || characterPrefabs.Length != buttonHandlers.Length) Debug.LogError("Characters Arrays Size Mismatch!");

            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                buttonHandlers[i].Initialize(this,i, characterPrefabs[i].Settings);
            }

            charactersPickStatus = new bool[characterPrefabs.Length];

            selectionPanel.SetActive(true);
            finishGameButton.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void EnableAllButtons(bool value)
        {
            foreach (var button in buttonHandlers)
            {
                button.EnableButton(value);
            }
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
                //TODO: Inform the player the character isn't available (in the chat) ORI
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
            obj.GetComponent<PlayableCharacterController>().Initialize("PLAYER NAME");//ORI
        }

    }
}


