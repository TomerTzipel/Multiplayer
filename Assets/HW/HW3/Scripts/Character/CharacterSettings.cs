using UnityEngine;

namespace HW3
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/Character/CharacterSettings3")]
    public class CharacterSettings : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Splash { get; private set; }
        //More in the future
    }
}


