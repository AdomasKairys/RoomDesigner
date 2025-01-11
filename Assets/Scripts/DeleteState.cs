using UnityEngine;

public class DeleteState : IBuildingState
{
    private int _placedObjectIndex = -1;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectManager _objectManager;

    public DeleteState(Grid grid,
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
        if (!IsPlacementValid() || isPointerOverUI) return;

        Debug.Log("surface " + surfaceDirection);
        // Scale the forward by -1 to get the direction of the cell behind
        _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);
         
        if(_placedObjectIndex == -1)
            return;
        _furnitureData.RemoveObjectByIndex(_placedObjectIndex);
        _objectManager.RemoveObjectAt(_placedObjectIndex);

    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        _previewSystem.UpdateIndicatorPosition(_grid.CellToWorld(gridPos), surfaceDirection);
        _previewSystem.UpdateIndicatorColor(IsPlacementValid());

        _placedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceDirection);
        _previewSystem.OutlineObject(_placedObjectIndex == -1 ? null : _objectManager.GetGameObjectByIndex(_placedObjectIndex));
    }
    private bool IsPlacementValid()
    {
        return InputManager.Instance.IsPointingAtActiveFurnitureObject(); //replace with bool
    }
}
