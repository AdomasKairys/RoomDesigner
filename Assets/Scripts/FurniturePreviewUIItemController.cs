using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
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
        LoadPreview(prefab.name);
    }
#if UNITY_EDITOR
    private void SavePreview(GameObject prefab)
    {
        string path = "Assets/Resources/Previews/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Texture2D preview = AssetPreview.GetAssetPreview(prefab);
        while(preview == null)
            preview = AssetPreview.GetAssetPreview(prefab);

        if (preview != null)
        {
            byte[] bytes = preview.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(path, prefab.name + ".png"), bytes);
            Debug.Log($"Saved preview for {prefab.name}");
        }
        AssetDatabase.Refresh();
    }
#endif
    private void LoadPreview(string objectName)
    {
        Texture2D previewTexture = Resources.Load<Texture2D>($"Previews/{objectName}");
        if (previewTexture != null)
        {
            _previewImage.texture = previewTexture;
        }
        else
            Debug.LogWarning($"Preview for {objectName} not found!");
    }
}
