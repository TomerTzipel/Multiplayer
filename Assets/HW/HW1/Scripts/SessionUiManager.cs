using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class SessionUiManager : MonoBehaviour
{
    [SerializeField] private SessionManager sessionManager;
    
    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text userCount;
    
    [SerializeField] private Transform usersScrollViewContent;
}