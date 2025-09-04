using UnityEngine;

[CreateAssetMenu(fileName = "CharactersRef", menuName = "Scriptable Objects/CharactersRef")]
public class CharactersRef : ScriptableObject
{
    [field: SerializeField] public PlayerCharacterController[] Characters;

    public Sprite GetCharacterSpriteAt(int index)
    {
        return Characters[index].Settings.Splash;
    }
}
