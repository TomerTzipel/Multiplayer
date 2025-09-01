using UnityEngine;

[CreateAssetMenu(fileName = "CharactersRef", menuName = "Scriptable Objects/CharactersRef")]
public class CharactersRef : ScriptableObject
{
    [field: SerializeField] public PlayerCharacterController[] Characters;
}
