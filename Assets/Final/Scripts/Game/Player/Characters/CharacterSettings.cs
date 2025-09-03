
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/Character/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Splash { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    [field: SerializeField] public float BaseSpeed { get; private set; } = 20;
    [field: SerializeField] public float AttackSpeed { get; private set; } = 1;
    [field: SerializeField] public int Damage { get; private set; } = 10;
    [field: SerializeField] public float ProjectileSpeed { get; private set; } = 5;
    [field: SerializeField] public float Range { get; private set; } = 10;
    [field: SerializeField,Range(0,100)] public int CritChance { get; private set; } = 10;
    [field: SerializeField] public ProjectileHandler ProjectilePrefab { get; private set; }

}




