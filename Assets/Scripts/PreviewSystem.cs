using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] float previewYOffset = 0.06f;

    [SerializeField] GameObject cellIndicator;
    [SerializeField] Material previewMaterialAsset;

    private Material _previewMaterialInstance;
    private GameObject _previewObject;
    private void Start()
    {
        cellIndicator.SetActive(false);
        _previewMaterialInstance = new Material(previewMaterialAsset);
    }

    public void ShowPlacementPreview(GameObject prefab, Vector3Int size)
    {
        cellIndicator.SetActive(true);
        _previewObject = Instantiate(prefab);
        ScaleCursor(size);
        EnablePreview(_previewObject);
    }
    public void HidePlacementPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(_previewObject);
    }
    public List<Vector3Int> UpdatePreviewPositions(Vector3 pos, Vector3 surfaceDirection, List<Vector3Int> shapeOffsets)
    {
        cellIndicator.transform.position = pos;
        cellIndicator.transform.LookAt(cellIndicator.transform.position - surfaceDirection);

        _previewObject.transform.position = new Vector3(pos.x, pos.y + previewYOffset, pos.z);

        if(_previewObject.transform.up == surfaceDirection) return shapeOffsets;

        _previewObject.transform.LookAt(_previewObject.transform.position + surfaceDirection);
        _previewObject.transform.RotateAround(_previewObject.transform.position, _previewObject.transform.right, 90);
        if (surfaceDirection != Vector3.up)
            _previewObject.transform.RotateAround(_previewObject.transform.position, _previewObject.transform.up, 180);


        List<Vector3Int> rotatedOffsets = new();
        for (int i = 0; i < shapeOffsets.Count; i++)
        {
            var rotatedOffset = _previewObject.transform.rotation * shapeOffsets[i];
            rotatedOffsets.Add(Vector3Int.RoundToInt(rotatedOffset));
            Debug.Log(rotatedOffset);
        }
        return rotatedOffsets;
    }
    public Vector3 GetAnchor()
    {
        return cellIndicator.transform.right - cellIndicator.transform.forward + cellIndicator.transform.up;
    }
    public void UpdatePreviewColor(bool isValid)
    {
        Color color = isValid ? Color.white : Color.red;
        color.a = 0.5f;
        cellIndicator.GetComponentInChildren<Renderer>().material.color = color;
        _previewMaterialInstance.color = color;
    }
    private void EnablePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
            renderer.materials = renderer.materials.Select((_) => _previewMaterialInstance).ToArray();
    }

    private void ScaleCursor(Vector3Int size)
    {
        if (size.x > 0 || size.z > 0)
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.z);
    }
}
