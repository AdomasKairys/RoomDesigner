using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveUIController : MonoBehaviour
{
    [SerializeField] RawImage thumbnailImage;
    [SerializeField] Transform scrollViewContentListTransfor;
    [SerializeField] GameObject saveDataItemPrefab;

    private Texture2D _thumbnailTexture;
    public void LoadSaveFileList()
    {
        thumbnailImage.gameObject.SetActive(false);
        var saveFileList = SaveManager.Instance.GetAllSaveFiles();
        foreach (var file in saveFileList) 
        {
            var saveFileItem = Instantiate(saveDataItemPrefab,scrollViewContentListTransfor);
            var itemController = saveFileItem.GetComponent<SaveListItemUIController>();
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
                SaveManager.Instance.LoadFurniture(file);
                gameObject.SetActive(false);
            });
            itemController.deleteButton.onClick.AddListener(() => 
            {
                thumbnailImage.gameObject.SetActive(false);
                SaveManager.Instance.DeleteSaveFile(file);
                Destroy(saveFileItem);
            });

        }
    }

}
