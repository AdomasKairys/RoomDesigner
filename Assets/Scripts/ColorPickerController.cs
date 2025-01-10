using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPickerController : MonoBehaviour
{
    public float currentHue, currentSaturation, currentValue;

    [SerializeField] RawImage hueImage, svImage, outputImage;

    [SerializeField] Slider hueSlider;

    [SerializeField] TMP_InputField hexInputField;

    private Texture2D _hueTexture, _svTexture, _outputTexture;

    public UnityEvent<Color> OnColorChange;

    private void Start()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
        UpdateOutputImage();
    }

    public void SetSV(float sat, float value)
    {
        currentSaturation = sat;
        currentValue = value;

        UpdateOutputImage();
    }
    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for (int y = 0; y < _svTexture.height; y++)
        {
            for (int x = 0; x < _svTexture.width; x++)
            {
                _svTexture.SetPixel(x, y,
                                    Color.HSVToRGB(currentHue,
                                                   (float)x / _svTexture.width,
                                                   (float)y / _svTexture.height));
            }
        }
        _svTexture.Apply();
        UpdateOutputImage();
    }
    public void OnTextInput()
    {
        if (hexInputField.text.Length < 6) return;

        Color newCol;

        if(ColorUtility.TryParseHtmlString("#"+ hexInputField.text, out newCol))
            Color.RGBToHSV(newCol, out currentHue, out currentSaturation, out currentValue);

        hueSlider.value = currentHue;
        hexInputField.text = "";
        UpdateOutputImage();
    }
    public void OnColorAccept()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSaturation, currentValue);
        OnColorChange?.Invoke(currentColor);
    }
    private void CreateHueImage()
    {
        _hueTexture = new Texture2D(1, 16);
        _hueTexture.wrapMode = TextureWrapMode.Clamp;
        _hueTexture.name = "HueTexture";

        for (int i = 0; i < _hueTexture.height; i++)
        {
            _hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / _hueTexture.height, 1, 1f));
        }
        _hueTexture.Apply();
        currentHue = 0;

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
                _svTexture.SetPixel(x, y,
                                    Color.HSVToRGB(currentHue,
                                                   (float)x / _svTexture.width,
                                                   (float)y / _svTexture.height));
            }
        }
        _svTexture.Apply();
        currentSaturation = 0;
        currentValue = 0;

        svImage.texture = _svTexture;
    }
    private void CreateOutputImage()
    {
        _outputTexture = new Texture2D(1, 16);
        _outputTexture.wrapMode = TextureWrapMode.Clamp;
        _outputTexture.name = "OutputTexture";

        Color currentColor = Color.HSVToRGB(currentHue, currentSaturation, currentValue);

        for (int i = 0; i < _hueTexture.height; i++)
        {
            _outputTexture.SetPixel(0, i, currentColor);
        }
        _outputTexture.Apply();

        outputImage.texture = _outputTexture;
    }
    private void UpdateOutputImage()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSaturation, currentValue);

        for (int i = 0; i < _hueTexture.height; i++)
        {
            _outputTexture.SetPixel(0, i, currentColor);
        }
        _outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColor);
    }
}
