using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public enum Team
{
    Red, Blue, Spectator
}


public struct PlayerData : INetworkStruct
{
    public NetworkString<_8> Name;
    public Team Team;
    public int CharacterIndex;
    public NetworkBool IsReady;   
}

public class SelectionNetworkManager : NetworkBehaviour
{
    public const int NO_CHARACTER = -1;
    [SerializeField] private GameNetworkManager gameManager;
    [SerializeField] private SelectionUIManager uiManager;
    [SerializeField] private NetworkRunnerRef networkRunnerRef;
 

    [Networked, Capacity(8),OnChangedRender(nameof(OnPlayerDataUpdate))]
    public NetworkDictionary<PlayerRef, PlayerData> PlayersData  => default;

    public override void Spawned()
    {
        OnPlayerDataUpdate();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestTeamChange_RPC(Team team, RpcInfo info = default)
    {
        int redPlayersCount = 0, bluePlayersCount = 0;

        foreach (var kvp in PlayersData)
        {
            if(kvp.Value.Team == Team.Blue)
                bluePlayersCount++;
            if (kvp.Value.Team == Team.Red)
                redPlayersCount++;
        }

        if((team == Team.Red && redPlayersCount == 2) || (team == Team.Blue && bluePlayersCount == 2))
        {
            FailedRequestResult_RPC(info.Source);
            return;
        }

        PlayerData data = PlayersData[info.Source];
        data.Team = team;
        data.CharacterIndex = NO_CHARACTER;
        data.IsReady = team == Team.Spectator;
        PlayersData.Set(info.Source, data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestReady_RPC(bool value, RpcInfo info = default)
    {
        PlayerData data = PlayersData[info.Source];
        if (data.CharacterIndex == NO_CHARACTER)
        {
            FailedRequestResult_RPC(info.Source);
            return;
        }

        data.IsReady = value;
        PlayersData.Set(info.Source, data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestCharacter_RPC(int characterIndex, RpcInfo info = default)
    {
        foreach (var kvp in PlayersData)
        {
            if (kvp.Value.CharacterIndex == characterIndex)
            {
                CharacterRequestResult_RPC(info.Source, false);
                return;
            }
        }

        PlayerData data = PlayersData[info.Source];
        data.CharacterIndex = characterIndex;
        PlayersData.Set(info.Source, data);
        CharacterRequestResult_RPC(info.Source, true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestRandomCharacter_RPC(RpcInfo info = default)
    {
        List<int> characterIndexList = new List<int>(10);
   
        for(var i = 0; i < 10; i++) characterIndexList.Add(i);
        foreach (var kvp in PlayersData)
        {
            if (kvp.Value.CharacterIndex != NO_CHARACTER) characterIndexList.Remove(kvp.Value.CharacterIndex);
        }
 
        PlayerData data = PlayersData[info.Source];
        data.CharacterIndex = characterIndexList[UnityEngine.Random.Range(0, characterIndexList.Count)];
        PlayersData.Set(info.Source, data);
        CharacterRequestResult_RPC(info.Source, true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestName_RPC(string name, RpcInfo info = default)
    {
        foreach (var kvp in PlayersData)
        {
            if (kvp.Value.Name == name)
            {
                NameRequestResult_RPC(info.Source, false);
                return;
            }
        }

        PlayerData data = PlayersData[info.Source];
        data.Name = name;
        PlayersData.Set(info.Source, data);
        NameRequestResult_RPC(info.Source, true);
    }
    
    public void OnSelectionLeave()
    {
        if (Runner.IsSharedModeMasterClient)
            foreach (var kvp in PlayersData)
            {
                if (kvp.Value.Name != PlayersData[Runner.LocalPlayer].Name) Runner.SetMasterClient(kvp.Key);
            }
        
        Runner.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnPlayerDataUpdate()
    {
        uiManager.UpdateUI(PlayersData);

        if (!Runner.IsSharedModeMasterClient)
        {
            uiManager.ShowStartGameButton(false);
            uiManager.EnableStartGameButton(false);
            return;
        }

        bool EnableStartGameButton = true;
        bool redPlayerExist = false;
        bool bluePlayerExist = false;
        foreach (var kvp in PlayersData)
        {
            if (!kvp.Value.IsReady)
            {
                EnableStartGameButton = false;
                break;
            }

            if (kvp.Value.Team == Team.Red) redPlayerExist = true;
            if (kvp.Value.Team == Team.Blue) bluePlayerExist = true;
        }

        if (!redPlayerExist || !bluePlayerExist) EnableStartGameButton = false;

        uiManager.ShowStartGameButton(true);
        uiManager.EnableStartGameButton(EnableStartGameButton);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void FailedRequestResult_RPC([RpcTarget] PlayerRef targetPlayer)
    {
        OnPlayerDataUpdate();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void CharacterRequestResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
    {
        if (result)
        {
            uiManager.EnableCharacterSelectionPanel(false);
        }
        else OnPlayerDataUpdate();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void NameRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool result)
    {
        
        if (result)
        {
            uiManager.EnableNameSelectionPanel(false);
        }
        else
        {
            OnPlayerDataUpdate();
            uiManager.EnableNameWarning(true);
        }
    }
}
