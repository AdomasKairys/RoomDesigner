using UnityEditor.Rendering;
using UnityEngine;

public class MoveState : IBuildingState
{

    private int _placedObjectIndex = -1;
    private int _selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectManager _objectManager;
    private Color _furnitureColor;

    private bool _isDragging = false;

    public MoveState(Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO objectDatabase,
                          GridData furnitureData,
                          ObjectManager objectManager)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _objectDatabase = objectDatabase;
        _furnitureData = furnitureData;
        _objectManager = objectManager;

        //InputManager.Instance.OnRotate.AddListener(() => { RotateObject(); });
        // Selected object to move
    }
    public void EndState()
    {
        _previewSystem.HidePreview();
        _previewSystem.OutlineObject(null);
        if(_placedObjectIndex != -1)
            _objectManager.ChangeObjectLayerByIndex(_placedObjectIndex, LayerMask.NameToLayer("Furniture"));
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        if (!_isDragging)
        {
            _isDragging = SelectObject(gridPos, surfaceDirection);
        }
        else
        {
            _isDragging = !MoveObject(gridPos,surfaceDirection);
        }
    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        _previewSystem.UpdateIndicatorPosition(_grid.CellToWorld(gridPos), surfaceDirection);
        if (!_isDragging)
        {
            _previewSystem.UpdateIndicatorColor(IsPlacementValid());
            _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);
            
            _previewSystem.OutlineObject(_placedObjectIndex == -1 ? null : _objectManager.GetGameObjectByIndex(_placedObjectIndex));
        }
        else
        {
            bool isPlacementValid = IsPlacementValid(gridPos, surfaceDirection);
            _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets = _previewSystem.UpdatePreviewPositions(_grid.CellToWorld(gridPos),
                                                                                                                          surfaceDirection,
                                                                                                                          _objectDatabase.objectsData[_selectedObjectIndex].ShapeOffsets) ??
                                                                                                                          _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets;
            _previewSystem.UpdatePreviewColor(isPlacementValid);
            _previewSystem.UpdateIndicatorColor(isPlacementValid);
        }
    }

    private bool SelectObject(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);
        _id = _furnitureData.GetId(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);

        _selectedObjectIndex = _objectDatabase.objectsData.FindIndex(x => x.Id == _id);
        if (_selectedObjectIndex < 0)
            return false; //throw exception

        _previewSystem.ShowPlacementPreview(
            _objectDatabase.objectsData[_selectedObjectIndex].Prefab,
            _objectDatabase.objectsData[_selectedObjectIndex].Size);

        _objectManager.ChangeObjectLayerByIndex(_placedObjectIndex, LayerMask.NameToLayer("FurnitureInactive"));
        _previewSystem.OutlineObject(_objectManager.GetGameObjectByIndex(_placedObjectIndex));
        _furnitureColor = _objectManager.GetColorByIndex(_placedObjectIndex);
        return true;
    }
    private bool MoveObject(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPlacementValid = IsPlacementValid(gridPos, surfaceDirection);

        if (!isPlacementValid || _placedObjectIndex == -1)
            return false;

        _previewSystem.HidePreview();
        _previewSystem.ShowSelectionCursor();

        _furnitureData.RemoveObjectByIndex(_placedObjectIndex);
        _objectManager.RemoveObjectAt(_placedObjectIndex);

        Transform previewTransform = _previewSystem.GetPreviewTransform();
        int index = _objectManager.PlaceObject(_objectDatabase.objectsData[_selectedObjectIndex].Prefab, previewTransform.position, previewTransform.rotation, _furnitureColor, _id);

        _furnitureData.AddObjectAt(gridPos,
            _objectDatabase.objectsData[_selectedObjectIndex].CurrentShapeOffsets ?? _objectDatabase.objectsData[_placedObjectIndex].ShapeOffsets,
            _previewSystem.GetDirectionToCellCenter(),
            _objectDatabase.objectsData[_selectedObjectIndex].Id,
            index);

        return true;

    }
    private bool IsPlacementValid()
    {
        return InputManager.Instance.IsPointingAtActiveFurnitureObject(); //replace with bool
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
                                             new int[]{ _placedObjectIndex});
    }
}
