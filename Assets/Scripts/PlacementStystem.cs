using System.Collections.Generic;
using UnityEngine;

// Wraps color struct to be passed by reference
public class ColorWrapper
{
    public Color color;
}
public class PlacementStystem : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] GameObject endStateButton;
    [SerializeField] ObjectDatabaseSO objectDatabase;
    [SerializeField] List<GameObject> grids;
    [SerializeField] PreviewSystem previewSystem;
    [SerializeField] ObjectManager objectManager;

    private GridData _furnitureData;

    private IBuildingState _buildingState;

    private ColorWrapper _furnitureColor = new();
    private void Start()
    {
        EndState();
        _furnitureData = new();
    }
    private void Update()
    {
        if (_buildingState == null) return;

        var (Position, SurfaceNormal) = InputManager.Instance.GetSelectedGridTilePosition();

        Vector3 mousePos = Position;
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        _buildingState.UpdateState(gridPos, SurfaceNormal);
    }
    public void ChangeMaterialColor(Color color)
    {
        _furnitureColor.color = color;
    }
    public GridData GetFurnitureData() => _furnitureData;
    public void LoadFurnitureData(SaveData data, ObjectDatabaseSO database) => _furnitureData = data.serializableGridData.ToGridData();
    public void ClearFurnitureData() => _furnitureData = new();
    public void StartPlacement(int Id)
    {
        EndState();
        endStateButton.SetActive(true);

        _buildingState = new PlacementState(Id,
                                            grid,
                                            previewSystem,
                                            objectDatabase,
                                            _furnitureData,
                                            objectManager,
                                            _furnitureColor);


        grids.ForEach(x => x.SetActive(true));
        InputManager.Instance.OnClick.AddListener(() => { ExecuteState(); });
        InputManager.Instance.OnExit.AddListener(() => { EndState(); });
    }
    public void StartPainting()
    {
        EndState();
        endStateButton.SetActive(true);

        _buildingState = new PaintingState(grid,
                                            previewSystem,
                                            objectDatabase,
                                            _furnitureData,
                                            objectManager,
                                            _furnitureColor);
        InputManager.Instance.useOverride = true;
        grids.ForEach(x => x.SetActive(true));
        InputManager.Instance.OnClick.AddListener(() => { ExecuteState(); });
        InputManager.Instance.OnExit.AddListener(() => { EndState(); });
    }
    public void StartMoving()
    {
        EndState();
        endStateButton.SetActive(true);

        _buildingState = new MoveState(grid,
                                            previewSystem,
                                            objectDatabase,
                                            _furnitureData,
                                            objectManager);

        InputManager.Instance.useOverride = true;
        grids.ForEach(x => x.SetActive(true));
        InputManager.Instance.OnClick.AddListener(() => { ExecuteState(); });
        InputManager.Instance.OnExit.AddListener(() => { EndState(); });
    }

    public void StartDelete()
    {
        EndState();
        endStateButton.SetActive(true);

        _buildingState = new DeleteState(grid,
                                         previewSystem,
                                         objectDatabase,
                                         _furnitureData,
                                         objectManager);
        InputManager.Instance.useOverride = true;
        grids.ForEach(x => x.SetActive(true));
        InputManager.Instance.OnClick.AddListener(() => { ExecuteState(); });
        InputManager.Instance.OnExit.AddListener(() => { EndState(); });
    }
    public void EndState()
    {
        InputManager.Instance.useOverride = false;
        grids.ForEach(x => x.SetActive(false));
        endStateButton.SetActive(false);

        if (_buildingState == null)
            return;

        _buildingState.EndState();
        InputManager.Instance.OnClick.RemoveAllListeners();
        InputManager.Instance.OnExit.RemoveAllListeners();
        InputManager.Instance.OnRotate.RemoveAllListeners();
        _buildingState = null;
    }
    private void ExecuteState()
    {
        var (Position, SurfaceNormal) = InputManager.Instance.GetSelectedGridTilePosition();
        Vector3 mousePos = Position;
        Vector3Int gridPos = grid.WorldToCell(mousePos);
        _buildingState.OnAction(gridPos, SurfaceNormal);
    }
}
