using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageController : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] Image cursorImage;

    private RawImage _svImage;

    private ColorPickerController _colorPickerController;

    private RectTransform  _rectTransform, _cursorTransform;

    private void Awake()
    {
        _svImage = GetComponent<RawImage>();
        _colorPickerController = FindFirstObjectByType<ColorPickerController>();
        _rectTransform = GetComponent<RectTransform>();

        _cursorTransform = cursorImage.GetComponent<RectTransform>();
        _cursorTransform.localPosition = new Vector2(-_rectTransform.sizeDelta.x * 0.5f, -_rectTransform.sizeDelta.y * 0.5f);
        cursorImage.color = Color.HSVToRGB(0, 0, 1);
        _colorPickerController.SetSV(0, 0);
        _colorPickerController.OnColorAccept();
    }

    public void UpdateColor(PointerEventData eventData)
    {
        Vector3 pos = _rectTransform.InverseTransformPoint(eventData.position);

        float deltaX = _rectTransform.sizeDelta.x * 0.5f;
        float deltaY = _rectTransform.sizeDelta.y * 0.5f;

        if(pos.x < -deltaX)
            pos.x = -deltaX;
        else if (pos.x > deltaX)
            pos.x = deltaX;

        if (pos.y < -deltaY)
            pos.y = -deltaY;
        else if (pos.y > deltaY)
            pos.y = deltaY;

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / _rectTransform.sizeDelta.x;
        float yNorm = y / _rectTransform.sizeDelta.y;

        _cursorTransform.localPosition = pos;
        cursorImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        _colorPickerController.SetSV(xNorm, yNorm);
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
