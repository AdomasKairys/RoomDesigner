using UnityEngine;

public class DeleteState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private ObjectDatabaseSO _objectDatabase;
    private GridData _furnitureData;
    private ObjectPlacer _objectPlacer;

    public DeleteState(Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO objectDatabase,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _objectDatabase = objectDatabase;
        _furnitureData = furnitureData;
        _objectPlacer = objectPlacer;

        previewSystem.ShowDeletePreview();
    }

    public void EndState()
    {
        _previewSystem.HidePlacementPreview();
    }

    public void OnAction(Vector3Int gridPos, Vector3 surfaceNormal)
    {
        bool isPointerOverUI = InputManager.Instance.IsPointerOverUI();
        if (!IsPlacementValid() || isPointerOverUI) return;

        Debug.Log("surface " + surfaceNormal);
        // Scale the forward by -1 to get the direction of the cell behind
        _selectedObjectIndex = _furnitureData.GetIndex(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceNormal);
         
        if(_selectedObjectIndex == -1)
            return;
        _furnitureData.RemoveObjectAt(gridPos, _previewSystem.GetDirectionToCellCenter() - 2 * surfaceNormal);
        _objectPlacer.RemoveObjectAt(_selectedObjectIndex);

    }
    public void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection)
    {
        _previewSystem.UpdateIndicatorPosition(_grid.CellToWorld(gridPos), surfaceDirection);
        _previewSystem.UpdateIndicatorColor(IsPlacementValid());
    }
    private bool IsPlacementValid()
    {
        return InputManager.Instance.GetSelectedFurnitureObjectPosition() != null; //replace with bool
    }
}
