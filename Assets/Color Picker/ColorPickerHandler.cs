using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer testing;

    [SerializeField] 
    private RawImage hueImage,satValImage,outputImage;

    [SerializeField] private Slider hueSlider;
    [SerializeField] private TMP_InputField hexInputField;

    private Texture2D _hueTexture, _svTexture, _outputTexture;
    private float _currentHue, _currentSat, _currentVal;
    
    public Color CurrentColor
    {
        get { return Color.HSVToRGB(_currentHue, _currentSat, _currentVal); }
    }

    private void Awake()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
    }

    public void SetSV(float S,float V)
    {
        _currentSat = S;
        _currentVal = V;

        UpdateOutputImage();
    }
    public void UpdateSVImage()
    {
        _currentHue = hueSlider.value;

        for (int y = 0; y < _svTexture.height; y++)
        {
            for (int x = 0; x < _svTexture.width; x++)
            {
                _svTexture.SetPixel(x, y, Color.HSVToRGB(_currentHue, (float)x / _svTexture.width, (float)y / _svTexture.height));
            }
        }

        _svTexture.Apply();

        UpdateOutputImage();
    }

    public void OnTextInput()
    {
        if(hexInputField.text.Length != 6) { return; }
        Color newColor;

        if (ColorUtility.TryParseHtmlString("#" + hexInputField.text, out newColor))
            Color.RGBToHSV(newColor, out _currentHue, out _currentSat, out _currentVal);

        hueSlider.value = _currentHue;

        hexInputField.text = "";

        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        _hueTexture = new Texture2D(1, 16);
        _hueTexture.wrapMode = TextureWrapMode.Clamp;
        _hueTexture.name = "HueTexture";

        for (int i = 0; i < _hueTexture.height; i++)
        {
            _hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / _hueTexture.height, 1f, 1f));
        }

        _hueTexture.Apply();
        _currentHue = 0;

        hueImage.texture = _hueTexture;
    }
    private void CreateSVImage()
    {
        _svTexture = new Texture2D(16, 16);
        _svTexture.wrapMode = TextureWrapMode.Clamp;
        _svTexture.name = "SatValTexture";

        for (int y = 0; y < _svTexture.height; y++)
        {
            for (int x = 0; x < _svTexture.width; x++)
            {
                _svTexture.SetPixel(x, y, Color.HSVToRGB(_currentHue,(float)x/_svTexture.width,(float)y/_svTexture.height));
            }
        }

        _svTexture.Apply();
        _currentSat = 0;
        _currentVal = 0;

        satValImage.texture = _svTexture;
    }

    private void CreateOutputImage()
    {
        _outputTexture = new Texture2D(1, 16);
        _outputTexture.wrapMode = TextureWrapMode.Clamp;
        _outputTexture.name = "OutputTexture";

        UpdateOutputImage();

        outputImage.texture = _outputTexture;
    }

    private void UpdateOutputImage()
    {
        for (int i = 0; i < _outputTexture.height; i++)
        {
            _outputTexture.SetPixel(0, i, CurrentColor);
        }
        _outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(CurrentColor);

        if(testing != null)
        {
            testing.material.SetColor("_BaseColor", CurrentColor);
        }   
    }
}
