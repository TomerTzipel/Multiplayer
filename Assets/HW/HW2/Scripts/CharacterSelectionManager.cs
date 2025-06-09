using Fusion;
using UnityEngine;

public class CharacterSelectionManager : NetworkBehaviour
{
    public override void Spawned()
    {
        Debug.Log("Spawned");
        base.Spawned();
    }
}
