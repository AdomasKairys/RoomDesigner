using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] GameObject cellIndicator;
    [SerializeField] Material previewMaterialAsset;

    private Material _previewMaterialInstance;
    private GameObject _previewObject;
    private GameObject _hoveredObject;
    private float _currentRotation = 0f;
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
    public void ShowSelectionCursor()
    {
        cellIndicator.SetActive(true);
        ScaleCursor(Vector3Int.one);
        UpdateIndicatorColor(false);
    }
    public void HidePreview()
    {
        cellIndicator.SetActive(false);
        if (_previewObject != null)
            Destroy(_previewObject);
    }
    public void UpdateIndicatorPosition(Vector3 pos, Vector3 surfaceDirection)
    {
        cellIndicator.transform.position = pos;
        cellIndicator.transform.LookAt(cellIndicator.transform.position - surfaceDirection);
    }
    public List<Vector3Int> UpdatePreviewPositions(Vector3 pos, Vector3 surfaceDirection, Vector3 pivot, List<Vector3Int> shapeOffsets)
    {
        _previewObject.transform.position = pos;

        if (_previewObject.transform.up != surfaceDirection)
        {

            _previewObject.transform.LookAt(_previewObject.transform.position + surfaceDirection);
            _previewObject.transform.RotateAround(_previewObject.transform.position, _previewObject.transform.right, 90);
            if (surfaceDirection != Vector3.up)
                _previewObject.transform.RotateAround(_previewObject.transform.position, _previewObject.transform.up, 180);
        }
        RotatePreviewBody(_currentRotation);
        Quaternion rotation = Quaternion.Euler(surfaceDirection * _currentRotation);
        List<Vector3Int> rotatedOffsets = RotateOffsets(shapeOffsets, _previewObject.transform.localRotation, new(-0.5f, -0.5f, -0.5f));// default pivot point
        return RotateOffsets(rotatedOffsets, rotation, pivot);
    }

    public void RotatePreview(float degrees = 90f)
    {
        _currentRotation = (_currentRotation + degrees) % 360;
    }
    public Transform GetPreviewTransform() 
    { 
        return _previewObject.transform; 
    }
    public Quaternion GetPreviewBodyRotation()
    {
        var previewBody = _previewObject.transform.Find("Body");
        if (previewBody == null)
        {
            Debug.LogError("Object doesn't have a Body child");
            return new();
        }
        return previewBody.rotation;
    }
    public Vector3 GetDirectionToCellCenter()
    {
        return cellIndicator.transform.right - cellIndicator.transform.forward + cellIndicator.transform.up;
    }
    public void UpdatePreviewColor(bool isValid)
    {
        Color color = isValid ? Color.white : Color.red;
        color.a = 0.5f;
        _previewMaterialInstance.color = color;
    }
    public void UpdateIndicatorColor(bool isValid)
    {
        Color color = isValid ? Color.white : Color.red;
        color.a = 0.5f;
        cellIndicator.GetComponentInChildren<Renderer>().material.color = color;
    }
    public void OutlineObject(GameObject gameObject)
    {
        if (_hoveredObject != null &&
            _hoveredObject != gameObject &&
            _hoveredObject.TryGetComponent(out Outline outline))
            outline.enabled = false;

        _hoveredObject = gameObject;

        if (_hoveredObject == null) return;

        if (!_hoveredObject.TryGetComponent(out outline))
            _hoveredObject.AddComponent<Outline>().OutlineWidth = 10;
        else
            outline.enabled = true;
    }
    private void RotatePreviewBody(float rotationDegrees)
    {
        var previewBody = _previewObject.transform.Find("Body");
        if (previewBody == null)
        {
            Debug.LogError("Object doesn't have a Body child");
            return;
        }
        previewBody.localRotation = Quaternion.Euler(0,rotationDegrees, 0);
    }
    private List<Vector3Int> RotateOffsets(List<Vector3Int> shapeOffsets, Quaternion rotation, Vector3 pivot)
    {
        List<Vector3Int> rotatedOffsets = new();
        for (int i = 0; i < shapeOffsets.Count; i++)
        {
            var realetiveOffset = shapeOffsets[i] - pivot;
            var rotatedOffset = rotation * realetiveOffset;
            rotatedOffsets.Add(Vector3Int.RoundToInt(rotatedOffset + pivot));
        }
        return rotatedOffsets;
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
            cellIndicator.transform.localScale = new Vector3(size.x, size.z, size.y);
    }

}
