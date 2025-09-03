using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerScoreHandler : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text killsCountText;
    [SerializeField] private TMP_Text deathCountText;

    public void InitializeUI(Sprite splashArt,string name)
    {
        characterImage.sprite = splashArt;
        nameText.text = name;
        UpdateUI(0, 0);
    }
    public void UpdateUI(int kills,int deaths)
    {
        killsCountText.text = kills.ToString("00");
        deathCountText.text = deaths.ToString("00");
    }
}
