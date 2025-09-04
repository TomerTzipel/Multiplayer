using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ScoreData : INetworkStruct
{
    public int Kills;
    public int Deaths;
    public Team Team;
}

public class PlayerScoreHandler : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text killsCountText;
    [SerializeField] private TMP_Text deathCountText;

    public string OwnerName { get; private set; }

    public void InitializeUI(string name,Sprite splashArt)
    {
        characterImage.sprite = splashArt;
        nameText.text = name;
        OwnerName = name;
        UpdateUI(0, 0);
    }
    public void UpdateUI(int kills,int deaths)
    {
        killsCountText.text = kills.ToString();
        deathCountText.text = deaths.ToString();
    }
}
