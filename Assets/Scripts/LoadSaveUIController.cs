using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveUIController : MonoBehaviour
{
    [SerializeField] RawImage thumbnailImage;
    [SerializeField] Transform scrollViewContentListTransform;
    [SerializeField] GameObject saveDataItemPrefab;

    [SerializeField] GameObject loadConfirmationPanel;
    [SerializeField] Button loadConfirmationButton;

    [SerializeField] GameObject deleteConfirmationPanel;
    [SerializeField] Button deleteConfirmationButton;

    private Texture2D _thumbnailTexture;
    private List<GameObject> _saveItems = new();
    private void OnEnable()
    {
        LoadSaveFileList();
    }
    private void OnDisable()
    {
        _saveItems.ForEach((s) => Destroy(s));
    }
    private void LoadSaveFileList()
    {
        thumbnailImage.gameObject.SetActive(false);
        var saveFileList = SaveManager.Instance.GetAllSaveFiles();
        foreach (var file in saveFileList) 
        {
            var saveFileItem = Instantiate(saveDataItemPrefab,scrollViewContentListTransform);
            _saveItems.Add(saveFileItem);
            if(!saveFileItem.TryGetComponent(out SaveListUIItemController itemController))
            {
                Debug.LogError("Save file prefab doesn't have a controller");
                return;
            }
            itemController.SetSaveFileData(file);
            itemController.OnHoveredEntered.AddListener(() =>
            {
                try
                {
                    thumbnailImage.gameObject.SetActive(true);
                    byte[] thumbnail = SaveManager.Instance.GetThumbnailBytes(file);
                    _thumbnailTexture = new(1, 1);
                    _thumbnailTexture.LoadImage(thumbnail);
                    thumbnailImage.texture = _thumbnailTexture;
                }
                catch 
                {
                    thumbnailImage.gameObject.SetActive(false);
                    Debug.LogError("Thumbnail not found for " + file);
                }
            });
            itemController.OnHoveredExit.AddListener(() => thumbnailImage.gameObject.SetActive(false));
            itemController.loadButton.onClick.AddListener(() => {
                loadConfirmationPanel.SetActive(true);
                loadConfirmationButton.onClick.RemoveAllListeners();
                loadConfirmationButton.onClick.AddListener(() => { 
                    SaveManager.Instance.LoadFurniture(file); });
            });
            itemController.deleteButton.onClick.AddListener(() => 
            {
                deleteConfirmationPanel.SetActive(true);
                deleteConfirmationButton.onClick.RemoveAllListeners();
                deleteConfirmationButton.onClick.AddListener(() =>
                {
                    thumbnailImage.gameObject.SetActive(false);
                    SaveManager.Instance.DeleteSaveFile(file);
                    Destroy(saveFileItem);
                    _saveItems.Remove(saveFileItem);
                });
            });

        }
    }

}
