using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveListUIItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI saveNameText;
    [SerializeField] TextMeshProUGUI saveDateText;
    public Button loadButton, deleteButton;

    private string _fileName;

    public UnityEvent OnHoveredEntered, OnHoveredExit;
    public void SetSaveFileData(string fileName)
    {
        _fileName = fileName;
        string[] split = Path.GetFileNameWithoutExtension(fileName).Split("_");
        saveNameText.text = split[0];
        saveDateText.text = split[1];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoveredEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoveredExit?.Invoke();
    }
}
