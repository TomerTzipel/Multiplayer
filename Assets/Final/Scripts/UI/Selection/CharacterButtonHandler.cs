using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image splashImage;
    [SerializeField] private SelectionUIManager uiManager;

    public int CharacterIndex { get; private set; }

    public event UnityAction<int> OnCharacterSelect; 

    public void Initialize(int index, string name, Sprite splash)
    {
        CharacterIndex = index;
        nameText.text = name;
        button.interactable = true;
        if (splash != null)
        {
            splashImage.sprite = splash;
        }
    }

    public void OnButtonClicked()
    {
        OnCharacterSelect.Invoke(CharacterIndex);
    }

    public void EnableButton(bool value)
    {
        button.interactable = value;
    }
}
