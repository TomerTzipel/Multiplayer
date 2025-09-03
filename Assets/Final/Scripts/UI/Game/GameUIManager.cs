using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionCanvas;
    [SerializeField] private GameObject gameCanvas;

    private void Awake()
    {
        gameCanvas.SetActive(false);
    }

    public void SetUpGameUI()
    {
        selectionCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
}
