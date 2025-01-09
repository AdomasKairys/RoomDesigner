using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    [SerializeField] Texture2D colorChart;
    [SerializeField] GameObject chart;
    [SerializeField] RectTransform cursor;
    [SerializeField] Image button;
    [SerializeField] Image cursorColor;

    public UnityEvent<Color> ColorPickerEvent;
    public void PickColor(BaseEventData data)
    {
        Vector2 chartPosition = chart.transform.position;
        Rect chartRect = chart.GetComponent<RectTransform>().rect;
        PointerEventData pointer = data as PointerEventData;
        Debug.Log(pointer.position + " " + chartPosition + " " + colorChart.width / chartRect.width);
        if (pointer.position.x < chartPosition.x || pointer.position.x > chartPosition.x + chartRect.width || pointer.position.y < chartPosition.y || pointer.position.y > chartPosition.y + chartRect.height)
            return;
        cursor.position = pointer.position;
        Color pickedColor = colorChart.GetPixel((int)(cursor.localPosition.x * (colorChart.width / chartRect.width)),
                                                (int)(cursor.localPosition.y * (colorChart.height / chartRect.height)));
        button.color = pickedColor;
        cursorColor.color = pickedColor;
        ColorPickerEvent.Invoke(pickedColor);
    }
}
