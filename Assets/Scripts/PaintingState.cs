using UnityEngine;

public class PaintingState : IBuildingState
{
    private int _placedObjectIndex = -1;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectManager _objectManager;
    private ColorWrapper _furnitureColor;


    public PaintingState(Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO objectDatabase,
                          GridData furnitureData,
                          ObjectManager objectManager,
                          ColorWrapper furnitureColor)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _objectDatabase = objectDatabase;
        _furnitureData = furnitureData;
        _objectManager = objectManager;
        _furnitureColor = furnitureColor;

        previewSystem.ShowSelectionCursor();
    }

    public void EndState()
    {
        _previewSystem.HidePreview();
        _previewSystem.OutlineObject(null);
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        bool isPointerOverUI = InputManager.Instance.IsPointerOverUI();
        if (!IsCursorOnObject() || isPointerOverUI) return;

        _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);

        if (_placedObjectIndex == -1)
            return;

        _objectManager.ChangeColorByIndex(_placedObjectIndex, _furnitureColor.color);
    }

    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        _previewSystem.UpdateIndicatorPosition(_grid.CellToWorld(gridPos), surfaceDirection);
        _previewSystem.UpdateIndicatorColor(IsCursorOnObject());

        _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);
        _previewSystem.OutlineObject(_placedObjectIndex == -1 ? null : _objectManager.GetGameObjectByIndex(_placedObjectIndex));
    }
    private bool IsCursorOnObject()
    {
        return InputManager.Instance.IsPointerOnActiveFurnitureObject(); //replace with bool
    }
}
