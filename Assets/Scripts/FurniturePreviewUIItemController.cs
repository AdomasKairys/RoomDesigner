using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FurniturePreviewUIItemController : MonoBehaviour
{
    private GameObject _furniturePrefab;
    private RawImage _previewImage;
    public void SetUpPreviewPrefab(GameObject prefab, Action onClick)
    {
        GetComponent<Button>().onClick.AddListener(()=> onClick());
        _previewImage = GetComponent<RawImage>();
        _furniturePrefab = prefab;
        StartCoroutine(LoadPreview());
    }
    private IEnumerator LoadPreview()
    {
        bool isPreviewLoaded = false;
        Texture2D previewTexture = null;
        while (!isPreviewLoaded || previewTexture == null)
        {
            isPreviewLoaded = !AssetPreview.IsLoadingAssetPreview(_furniturePrefab.GetInstanceID());
            previewTexture = AssetPreview.GetAssetPreview(_furniturePrefab);
            yield return null;
        }
        _previewImage.texture = previewTexture;
    }
}
