using System.Collections.Generic;
using UnityEngine;

public class PlacementStystem : MonoBehaviour
{
    [SerializeField] Grid grid;

    [SerializeField] ObjectDatabaseSO objectDatabase;
    [SerializeField] List<GameObject> grids;
    [SerializeField] PreviewSystem previewSystem;
    [SerializeField] ObjectPlacer objectPlacer;

    private GridData furnitureData;

    private IBuildingState _buildingState;

    private void Start()
    {
        StopPlacement();
        furnitureData = new();
    }
    private void Update()
    {
        if (_buildingState == null) return;

        var tilePos = InputManager.Instance.GetSelectedGridTilePosition();
        Vector3 mousePos = tilePos.Position;
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        _buildingState.UpdateState(gridPos, tilePos.SurfaceNormal);
    }

    public void StartPlacement(int Id)
    {
        StopPlacement();
        
        _buildingState = new PlacementState(Id,
                                            grid,
                                            previewSystem,
                                            objectDatabase,
                                            furnitureData,
                                            objectPlacer);

        grids.ForEach(x => x.SetActive(true));
        InputManager.Instance.OnClick.AddListener(() => { PlaceObject(); });
        InputManager.Instance.OnExit.AddListener(() => { StopPlacement(); });
    }
    private void PlaceObject()
    {
        var tilePos = InputManager.Instance.GetSelectedGridTilePosition();
        Vector3 mousePos = tilePos.Position;
        Vector3Int gridPos = grid.WorldToCell(mousePos);
        _buildingState.OnAction(gridPos, tilePos.SurfaceNormal);
    }

    private void StopPlacement()
    {
        grids.ForEach(x => x.SetActive(false));
        
        if (_buildingState == null)
            return;
        _buildingState.EndState();
        InputManager.Instance.OnClick.RemoveAllListeners();
        InputManager.Instance.OnExit.RemoveAllListeners();
        InputManager.Instance.OnRotate.RemoveAllListeners();
        _buildingState = null;
    }


}
