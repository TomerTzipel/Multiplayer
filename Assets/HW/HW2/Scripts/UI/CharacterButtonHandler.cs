
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HW2
{
    public class CharacterButtonHandler : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image splashImage;

        private CharacterSelectionManager _manager;
        private int _characterIndex;
        public void Initialize(CharacterSelectionManager manager, int index, CharacterSettings settings)
        {
            _manager = manager;
            _characterIndex = index;
            nameText.text = settings.Name;

            if (settings.Splash != null)
            {
                splashImage.sprite = settings.Splash;
            }
        }

        public void OnButtonClicked()
        {
            _manager.EnableAllButtons(false);
            _manager.RequestCharacter_RPC(_characterIndex);
        }

        public void EnableButton(bool value)
        {
            button.interactable = value;
        }
    }

}
