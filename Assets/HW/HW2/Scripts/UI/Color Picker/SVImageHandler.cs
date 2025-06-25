using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageHandler : MonoBehaviour, IDragHandler,IPointerClickHandler
{
    
    [SerializeField] private Image pickerImage;

    [SerializeField] private RawImage SVImage;

    [SerializeField] private ColorPickerHandler colorPickerHandler;

    [SerializeField] private RectTransform rectTransform,pickerTransform;

    private void Awake()
    {
        pickerTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    }

    private void UpdateColor(PointerEventData eventData)
    {
        Vector3 position = rectTransform.InverseTransformPoint(eventData.position);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        position.x = Mathf.Clamp(position.x, -deltaX, deltaX);
        position.y = Mathf.Clamp(position.y, -deltaY, deltaY);

        float x = position.x + deltaX;
        float y = position.y + deltaY;

        float xNorm = x / rectTransform.sizeDelta.x;
        float yNorm = y / rectTransform.sizeDelta.y;

        pickerTransform.localPosition = position;
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);
        colorPickerHandler.SetSV(xNorm, yNorm);
    }
    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }
}
