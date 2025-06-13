using Fusion;
using UnityEngine;

namespace HW2
{
    public class CharacterSelectionManager : NetworkBehaviour
    {
        [SerializeField] private PlayableCharacterController[] characterPrefabs;
        [SerializeField] private Transform[] characterSpawnPositions;
        [SerializeField] private CharacterButtonHandler[] buttonHandlers;
        [SerializeField] private GameObject selectionPanel;
        private bool[] charactersPickStatus;

        public override void Spawned()
        { 
            Debug.Log("Spawned");

            if (characterPrefabs.Length != characterSpawnPositions.Length || characterPrefabs.Length != buttonHandlers.Length) Debug.LogError("Characters Arrays Size Mismatch!");

            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                buttonHandlers[i].Initialize(this,i, characterPrefabs[i].Settings);
            }

            charactersPickStatus = new bool[characterPrefabs.Length];
            selectionPanel.SetActive(true);
        }

        [Rpc (RpcSources.All,RpcTargets.StateAuthority)]

        public void RequestCharacter_RPC(int characterIndex,RpcInfo info = default)
        {
            Debug.Log($"Character request from {info.Source.PlayerId}");
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
                //Inform the player the character isn't available
                Debug.Log("ALREADY TAKEN BE FASTER");
                return;
            }

            //Spawn the correct prefab at the correct position
            PlayableCharacterController characterPrefab = characterPrefabs[characterIndex];
            Vector3 position = characterSpawnPositions[characterIndex].position;
            NetworkManager.Instance.NetworkRunner.Spawn(characterPrefab, position);
            Debug.Log($"Spawned character {characterIndex} at {position}");
            selectionPanel.SetActive(false);
        }
    }
}


