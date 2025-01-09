using UnityEngine;

public class MoveState : IBuildingState
{

    private int _selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectPlacer _objectPlacer;

    public MoveState(int id,
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

        // Selected object to move


    }
    public void EndState()
    {
        _previewSystem.HidePlacementPreview();
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos);

        if (!isPlacementValid) return;



        int index = _objectPlacer.PlaceObject(_objectDatabase.objectsData[_selectedObjectIndex].Prefab, _previewSystem.GetPreviewTransform());

        _furnitureData.AddObjectAt(gridPos,
            _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
            _previewSystem.GetDirectionToCellCenter(),
            _objectDatabase.objectsData[_selectedObjectIndex].Id,
            index);
    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos);
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
    private bool IsPlacementValid(Vector3Int gridPos)
    {
        GridData selectedData = _furnitureData; //change for object on object
        return selectedData.CanPlaceObjectAt(gridPos,
                                             _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets,
                                             _previewSystem.GetDirectionToCellCenter());
    }
}
