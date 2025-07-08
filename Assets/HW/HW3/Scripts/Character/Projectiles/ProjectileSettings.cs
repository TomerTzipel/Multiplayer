using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSettings", menuName = "Scriptable Objects/Projectile/ProjectileSettings")]
public class ProjectileSettings : ScriptableObject
{
    [field:SerializeField] public float Speed {  get; private set; }
    [field: SerializeField] public float Lifetime { get; private set; }
}
