using System;
using TMPro;
using UnityEngine;


public class DirectJoinUIHandler : MonoBehaviour
{
    [SerializeField] private MainMenuNetworkManager networkManager;

    [SerializeField] private GameObject nameWarningText;
    [SerializeField] private TMP_InputField nameInputField;

    private void Awake()
    {
        nameWarningText.SetActive(false);
    }

    public void JoinSession()
    {
        nameWarningText.SetActive(false);
        bool result = networkManager.JoinSession(nameInputField.text);

        if(!result) nameWarningText.SetActive(true);
    }

}
