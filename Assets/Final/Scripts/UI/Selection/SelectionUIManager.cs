using Fusion;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUIManager : MonoBehaviour
{
    [SerializeField] private SelectionNetworkManager networkManager;
    [SerializeField] private CharacterSettings[] characterSettings;
    [SerializeField] private CharacterButtonHandler[] buttonHandlers;


    [SerializeField] private PlayerStatusHandler[] redTeamPlayerStatusHandlers;
    [SerializeField] private PlayerStatusHandler[] blueTeamPlayerStatusHandlers;
    [SerializeField] private TMP_Text spectatorsListText;

    [SerializeField] private Button startGameButton;

    [SerializeField] private Button joinRedButton;
    [SerializeField] private Button joinBlueButton;
    [SerializeField] private Button joinSpectatorsButton;

    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject nameSelectionPanel;
    [SerializeField] private GameObject nameWarningText;
    [SerializeField] private TMP_InputField nameInputField;
    private PlayerRef LocalPlayer { get {  return networkManager.Runner.LocalPlayer; } }

    private void Awake()
    {
        startGameButton.gameObject.SetActive(false);
        EnableNameSelectionPanel(true);
        EnableNameWarning(false);
        EnableCharacterSelectionPanel(false);

        for (int i = 0; i < buttonHandlers.Length; i++)
        {
            buttonHandlers[i].OnCharacterSelect += HandleCharacterSelect;
            buttonHandlers[i].Initialize(i, characterSettings[i].name, characterSettings[i].Splash);
        }

        foreach (PlayerStatusHandler handler in redTeamPlayerStatusHandlers)
        {
            handler.OnReadyClick += HandleReadyButton;
        }
        foreach (PlayerStatusHandler handler in blueTeamPlayerStatusHandlers)
        {
            handler.OnReadyClick += HandleReadyButton;
        }
    }

    public void EnableAllButtons(bool value)
    {
        foreach (CharacterButtonHandler buttonHandler in buttonHandlers)
        {
            buttonHandler.EnableButton(value);
        }
        foreach (PlayerStatusHandler handler in redTeamPlayerStatusHandlers)
        {
            handler.EnableButtons(value);
        }
        foreach (PlayerStatusHandler handler in blueTeamPlayerStatusHandlers)
        {
            handler.EnableButtons(value);
        }

        startGameButton.interactable = value;
        joinRedButton.interactable = value;
        joinBlueButton.interactable = value;
        joinSpectatorsButton.interactable = value;
    }

    public void OnPlayerJoinTeam(int team)
    {
        EnableAllButtons(false);
        networkManager.RequestTeamChange_RPC((Team)team,LocalPlayer);
    }
    public void HandleReadyButton(bool value)
    {
        EnableAllButtons(false);
        networkManager.RequestReady_RPC(value, LocalPlayer);
    }
    public void UpdateUI(NetworkDictionary<PlayerRef, PlayerData> playersData)
    {
        EnableAllButtons(true);
        List<string> spectators = new List<string>(8);
        int redPlayersIndex = 0, bluePlayersIndex = 0;


        foreach (PlayerStatusHandler handler in redTeamPlayerStatusHandlers)
        {
            handler.ResetHandler();
        }
        foreach (PlayerStatusHandler handler in blueTeamPlayerStatusHandlers)
        {
            handler.ResetHandler();
        }

        foreach (var kvp in playersData)
        {
            switch (kvp.Value.Team)
            {
                case Team.Red:
                    redTeamPlayerStatusHandlers[redPlayersIndex].UpdateUI(kvp.Key, kvp.Value, LocalPlayer, characterSettings);
                    joinRedButton.interactable = kvp.Key != LocalPlayer;
                    redPlayersIndex++;
                    break;
                case Team.Blue:
                    blueTeamPlayerStatusHandlers[bluePlayersIndex].UpdateUI(kvp.Key, kvp.Value, LocalPlayer, characterSettings);
                    joinBlueButton.interactable = kvp.Key != LocalPlayer;
                    bluePlayersIndex++;
                    break;
                case Team.Spectator:
                    spectators.Add((string)kvp.Value.Name);
                    joinSpectatorsButton.interactable = kvp.Key != LocalPlayer;
                    break;
            }

            foreach (CharacterButtonHandler handler in buttonHandlers)
            {
                if (handler.CharacterIndex == kvp.Value.CharacterIndex)
                    handler.EnableButton(false);
            }

        }
        UpdateSpectatorList(spectators);

        joinRedButton.gameObject.SetActive(redPlayersIndex < 2);
        joinBlueButton.gameObject.SetActive(redPlayersIndex < 2);
    }
    public void EnableStartGameButton(bool value)
    {
        startGameButton.gameObject.SetActive(true);
        startGameButton.interactable = value;
    }

    public void EnableNameSelectionPanel(bool value)
    {
        nameSelectionPanel.SetActive(value);
    }
    public void EnableNameWarning(bool value)
    {
        nameWarningText.SetActive(value);
    }
    public void EnableCharacterSelectionPanel(bool value)
    {
        characterSelectionPanel.SetActive(value);
    }
    public void HandleNameConfirmButton()
    {
        EnableNameWarning(false);
        SelectName(nameInputField.text);
    }

    public void SelectName(string name)
    {
        EnableAllButtons(false);
        networkManager.RequestName_RPC(name,LocalPlayer);
    }
    private void HandleCharacterSelect(int characterIndex)
    {
        EnableAllButtons(false);
        networkManager.RequestCharacter_RPC(characterIndex, LocalPlayer);
    }
    
    private void UpdateSpectatorList(List<string> names)
    {
        string text = "";

        if(names.Count == 0)
        {
            spectatorsListText.text = text;
            return;
        }

        foreach (string name in names)
        {
            text += name + ", ";
        }
        text = text.Substring(0,text.Length-2);
        spectatorsListText.text = text;
    }
}
