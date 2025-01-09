using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask surfaceLayerMask;

    private (Vector3 Position, Vector3 SurfaceNormal) _lastPosition;

    public UnityEvent OnClick, OnExit;

    public static InputManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClick?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }
    public (Vector3 Position, Vector3 SurfaceNormal) GetSelectedGridTilePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, surfaceLayerMask))
        {
            _lastPosition.Position = hit.point;
            _lastPosition.SurfaceNormal = hit.normal;
        }
        return _lastPosition;
    }
}
