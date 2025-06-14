using UnityEngine;

namespace HW2
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/Character/CharacterSettings")]
    public class CharacterSettings : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Splash { get; private set; }
        //More in the future
    }
}


