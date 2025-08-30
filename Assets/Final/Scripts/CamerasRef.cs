using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CamerasRef", menuName = "Scriptable Objects/CamerasRef")]
public class CamerasRef : ScriptableObject
{
    public Camera MainCamera { get; set; }
    public CinemachineCamera CineCam { get; set; }
}
