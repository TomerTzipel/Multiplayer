using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ScoreData : INetworkStruct
{
    public int charachterIndex;
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

    public void UpdateUI(string name, Sprite splashArt, int kills,int deaths)
    {
        nameText.text = name;
        characterImage.sprite = splashArt;
        killsCountText.text = kills.ToString();
        deathCountText.text = deaths.ToString();
    }
}
