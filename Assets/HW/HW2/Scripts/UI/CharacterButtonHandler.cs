using HW2;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image splashImage;

    private CharacterSelectionManager _manager;
    private int _characterIndex;
    public void Initialize(CharacterSelectionManager manager, int index, CharacterSettings settings)
    {
        Debug.Log(settings == null);
        _manager = manager;
        _characterIndex = index;
        nameText.text = settings.Name;

        if(settings.Splash != null)
        {
            splashImage.sprite = settings.Splash;
        }       
    }

    public void OnButtonClicked()
    {
        _manager.RequestCharacter_RPC(_characterIndex);
    }

    public void EnableButton()
    {
        button.interactable = true;
    }
    public void DisableButton()
    {
        button.interactable = false;
    }
}
