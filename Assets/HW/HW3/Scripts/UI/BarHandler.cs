using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarHandler : MonoBehaviour
{
    //Serialized Fields:
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text sliderText;

    public void UpdateSlider(float value, int currentHP, int maxHP)
    {
        slider.value = Mathf.Clamp(value, 0f, 1f);
        sliderText.text = $"{currentHP}/{maxHP}";
    }

    public void FillInDuration(float duration)
    {
        slider.value = 0;
        StartCoroutine(FillBar(duration));
    }

    private IEnumerator FillBar(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            yield return null;
            slider.value = time / duration;
            time += Time.deltaTime;
        }
    }
}
