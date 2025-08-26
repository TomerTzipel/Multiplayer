using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class PlayerStatusHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image readyStatusImage;
    [SerializeField] private Image characterSplashImage;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button characterButton;

    public event UnityAction<bool> OnReadyClick;

    public void ResetHandler()
    {
        gameObject.SetActive(false);
        readyStatusImage.color = Color.red;
        characterSplashImage.sprite = null;
    }

    public void UpdateUI(PlayerRef player,PlayerData data,PlayerRef localPlayer, CharacterSettings[] characters)
    {
        gameObject.SetActive(true);
        if (data.IsReady) readyStatusImage.color = Color.green;
        else readyStatusImage.color = Color.red;

        playerNameText.text = (string)data.Name;
        readyButton.interactable = player == localPlayer;
        characterButton.interactable = player == localPlayer;

        if(data.CharacterIndex >= 0)
        {
            characterSplashImage.sprite = characters[data.CharacterIndex].Splash;
        }    
    }

    public void EnableButtons(bool value)
    {
        readyButton.interactable = value;
        characterButton.interactable = value;
    }

    public void OnReadyButtonClick()
    {
        OnReadyClick.Invoke(readyStatusImage.color != Color.green);
    }
}
