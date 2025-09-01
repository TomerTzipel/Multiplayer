using UnityEngine;
using Fusion;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NetworkRunnerRef", menuName = "Scriptable Objects/NetworkRunnerRef")]
public class NetworkRunnerRef : ScriptableObject
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    public NetworkRunner CurrentNetworkRunner { get; private set; }

    public Dictionary<PlayerRef, PlayerData> PlayerData { get; set; }

    public void GenerateRunner(INetworkRunnerCallbacks networkManager)
    {
        GenerateNewRunner();
        CurrentNetworkRunner.AddCallbacks(networkManager);
        CurrentNetworkRunner.ProvideInput = true;
    }

    public void AddCallbacks(INetworkRunnerCallbacks networkManager)
    {
        CurrentNetworkRunner.AddCallbacks(networkManager);
    }
    
    public void RemoveCallbacks(INetworkRunnerCallbacks networkManager)
    {
        CurrentNetworkRunner.RemoveCallbacks(networkManager);
    }
    
    public void GenerateNewRunner()
    {
        if (CurrentNetworkRunner != null) Destroy(CurrentNetworkRunner.gameObject);

        CurrentNetworkRunner = Instantiate(networkRunnerPrefab);
    }
}
