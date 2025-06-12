using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/Character/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }

    //More in the future
}
