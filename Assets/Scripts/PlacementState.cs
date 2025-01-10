using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectPlacer _objectPlacer;
    private Color _furnitureColor;

    public UnityEvent<Color> OnColorChanged = new UnityEvent<Color>();

    public PlacementState(int id,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO objectDatabase,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        _id = id;
        _grid = grid;
        _previewSystem = previewSystem;
        _objectDatabase = objectDatabase;
        _furnitureData = furnitureData;
        _objectPlacer = objectPlacer;

        _selectedObjectIndex = _objectDatabase.objectsData.FindIndex(x => x.Id == _id);
        if (_selectedObjectIndex < 0)
            return; //throw exception

        previewSystem.ShowPlacementPreview(
            _objectDatabase.objectsData[_selectedObjectIndex].Prefab,
            _objectDatabase.objectsData[_selectedObjectIndex].Size);

        InputManager.Instance.OnRotate.AddListener(() => { RotateObject(); });
        OnColorChanged.AddListener((color)  => _furnitureColor = color);
    }


    public void EndState()
    {
        _previewSystem.HidePlacementPreview();
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos, surfaceDirection);
        bool isPointerOverUI = InputManager.Instance.IsPointerOverUI();
        if (!isPlacementValid || isPointerOverUI) return;


        int index = _objectPlacer.PlaceObject(_objectDatabase.objectsData[_selectedObjectIndex].Prefab, _previewSystem.GetPreviewTransform(), _furnitureColor,_id);

        _furnitureData.AddObjectAt(gridPos,
            _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
            _previewSystem.GetDirectionToCellCenter(),
            _objectDatabase.objectsData[_selectedObjectIndex].Id,
            index);
    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos, surfaceDirection);
        _previewSystem.UpdateIndicatorPosition(_grid.CellToWorld(gridPos), surfaceDirection);
        _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets = _previewSystem.UpdatePreviewPositions(_grid.CellToWorld(gridPos),
                                                                                                                      surfaceDirection,
                                                                                                                      _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets) ??
                                                                                                                      _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets;
        _previewSystem.UpdatePreviewColor(isPlacementValid);
        _previewSystem.UpdateIndicatorColor(isPlacementValid);
    }
    private void RotateObject()
    {
       _previewSystem.RotatePreview();
    }
    private bool IsPlacementValid(Vector3Int gridPos, Vector3 surfaceDirection)
    {

        var surfaceNormals = _objectDatabase.objectsData[_selectedObjectIndex].PlacableSurfaceNormals;

        if ((surfaceNormals.x < 1 || surfaceDirection.x < 1) &&
            (surfaceNormals.y < 1 || surfaceDirection.y < 1) &&
            (surfaceNormals.z < 1 || surfaceDirection.z < 1))
            return false;

        GridData selectedData = _furnitureData; //change for object on object
        return selectedData.CanPlaceObjectAt(gridPos,
                                             _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
                                             _previewSystem.GetDirectionToCellCenter());
    }
}
