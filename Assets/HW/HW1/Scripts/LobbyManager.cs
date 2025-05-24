using Fusion;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunner;

    public void StartSession(string sessionName)
    {
        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            OnGameStarted = OnGameStarted
        });
    }

    private void OnGameStarted(NetworkRunner runner)
    {
        Debug.Log($"{runner.SessionInfo.Name} Started");
    }
}
