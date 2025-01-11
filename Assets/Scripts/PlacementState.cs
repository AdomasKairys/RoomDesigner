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
    private ObjectManager _objectManager;
    private ColorWrapper _furnitureColor;

    public PlacementState(int id,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO objectDatabase,
                          GridData furnitureData,
                          ObjectManager objectManager,
                          ColorWrapper furnitureColor)
    {
        _id = id;
        _grid = grid;
        _previewSystem = previewSystem;
        _objectDatabase = objectDatabase;
        _furnitureData = furnitureData;
        _objectManager = objectManager;
        _furnitureColor=furnitureColor;

        _selectedObjectIndex = _objectDatabase.objectsData.FindIndex(x => x.Id == _id);
        if (_selectedObjectIndex < 0)
            return; //throw exception

        previewSystem.ShowPlacementPreview(
            _objectDatabase.objectsData[_selectedObjectIndex].Prefab,
            _objectDatabase.objectsData[_selectedObjectIndex].Size);

        // Rotation doesn't work, needs debugging
        //InputManager.Instance.OnRotate.AddListener(() => { RotateObject(); });
    }


    public void EndState()
    {
        _previewSystem.HidePreview();
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos, surfaceDirection);
        bool isPointerOverUI = InputManager.Instance.IsPointerOverUI();
        if (!isPlacementValid || isPointerOverUI) return;

        Transform previewTransform = _previewSystem.GetPreviewTransform();
        int index = _objectManager.PlaceObject(_objectDatabase.objectsData[_selectedObjectIndex].Prefab, previewTransform.position, previewTransform.rotation, _furnitureColor.color,_id);

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
        var placableAxis = _objectDatabase.objectsData[_selectedObjectIndex].PlacableSurfaceAxis;
        var surfaceDirectionRounded = Vector3Int.RoundToInt(surfaceDirection);
        if ((surfaceDirectionRounded.x == 0 || placableAxis.x == 0) &&
            (surfaceDirectionRounded.y == 0 || placableAxis.y == 0) &&
            (surfaceDirectionRounded.z == 0 || placableAxis.z == 0))
            return false;

        GridData selectedData = _furnitureData; //change for object on object
        return selectedData.CanPlaceObjectAt(gridPos,
                                             _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
                                             _previewSystem.GetDirectionToCellCenter(),
                                             new int[] { });
    }
}
