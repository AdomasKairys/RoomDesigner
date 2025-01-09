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
        throw new System.NotImplementedException();
    }

    public void OnAction(Vector3Int gridPos, Vector3 forward)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState(Vector3Int gridPos, Vector3 forward)
    {
        throw new System.NotImplementedException();
    }
}
