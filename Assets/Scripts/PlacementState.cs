using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectPlacer _objectPlacer;

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

    }
    public void EndState()
    {
        _previewSystem.HidePlacementPreview();
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos);

        if (!isPlacementValid) return;

        var placedObjInfo = _objectPlacer.PlaceObject(_objectDatabase.objectsData[_selectedObjectIndex].Prefab,
                                                      _grid.CellToWorld(gridPos),
                                                      surfaceDirection,
                                                      _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets);

        _furnitureData.AddObjectAt(gridPos,
            placedObjInfo.FinalOffset,
            _previewSystem.GetAnchor(),
            _objectDatabase.objectsData[_selectedObjectIndex].Id,
            placedObjInfo.Index);
    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos);
        _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets = _previewSystem.UpdatePreviewPositions(_grid.CellToWorld(gridPos),
                                                                                                                      surfaceDirection,
                                                                                                                      _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets);
        _previewSystem.UpdatePreviewColor(isPlacementValid);
    }
    private bool IsPlacementValid(Vector3Int gridPos)
    {
        GridData selectedData = _furnitureData; //change for object on object
        return selectedData.CanPlaceObjectAt(gridPos,
                                             _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
                                             _previewSystem.GetAnchor());
    }
}
