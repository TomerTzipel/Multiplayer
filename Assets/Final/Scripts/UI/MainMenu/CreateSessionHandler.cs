using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateSessionHandler : MonoBehaviour
{

    public const int MIN_PLAYERS = 4;
    public const int MAX_PLAYERS = 8;

    [SerializeField] private MainMenuNetworkManager networkManager;

    [SerializeField] private GameObject nameWarningText;
    [SerializeField] private TMP_Text countWarningText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField playerCountInputField;
    [SerializeField] private Toggle visibilityToggle;

    private void Awake()
    {
        nameWarningText.SetActive(false);
        countWarningText.text = $"*Player Count must be {MIN_PLAYERS}-{MAX_PLAYERS}!";
    }

    public void CreateSession()
    {
        if (playerCountInputField.text == string.Empty) return;

        int playerCount = int.Parse(playerCountInputField.text);
        if (playerCount < MIN_PLAYERS || playerCount > MAX_PLAYERS) return;

        nameWarningText.SetActive(false);
        bool result = networkManager.CreateSession(nameInputField.text, visibilityToggle.isOn, playerCount);

        if (!result) nameWarningText.SetActive(true);
    }
}
