using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool useOverride = false;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask surfaceLayerMask;
    [SerializeField] LayerMask furnitureLayerMask;
    [SerializeField] InputActionReference exitInput;
    [SerializeField] InputActionReference primaryInput;
    [SerializeField] InputActionReference secondaryInput;



    private (Vector3 Position, Vector3 SurfaceNormal) _lastPosition;
    private LayerMask _collisionLayerMask;
    private LayerMask _overrideCollisionLayerMask;


    public static InputManager Instance { get; private set; }

    public UnityEvent OnClick, OnExit, OnRotate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
        _collisionLayerMask = surfaceLayerMask;
        _lastPosition.Position = new Vector3(0,0,-5);
        _lastPosition.SurfaceNormal = new Vector3(0, 1, 0);
        _overrideCollisionLayerMask = surfaceLayerMask+furnitureLayerMask;

        exitInput.action.performed += OnExitPreformed;
        primaryInput.action.performed += OnClickPreformed;
        secondaryInput.action.performed += OnRotatePreformed;
    }

    public bool IsPointerOverUI()
    {
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
            return true;
        return false;
    }
    public void ToggleFurnitureLayerMaskInclusion()
    {
        _collisionLayerMask ^= furnitureLayerMask;
    }
    public bool IsPointerOnActiveFurnitureObject()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out _, 100, furnitureLayerMask))
            return true;

        return false;
    }
    public (Vector3 Position, Vector3 SurfaceNormal) GetSelectedGridTilePosition()
    {
        LayerMask mask = _collisionLayerMask;
        if (useOverride)
            mask=_overrideCollisionLayerMask;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            _lastPosition.Position = hit.point;
            _lastPosition.SurfaceNormal = hit.normal;
        }
        return _lastPosition;
    }
    private void OnExitPreformed(InputAction.CallbackContext context)
    {
        if(context.action.IsPressed())
            OnExit?.Invoke();
    }
    private void OnClickPreformed(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
            OnClick?.Invoke();
    }
    private void OnRotatePreformed(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
            OnRotate?.Invoke();
    }
    private void OnDestroy()
    {
        exitInput.action.performed -= OnExitPreformed;
        primaryInput.action.performed -= OnClickPreformed;
        secondaryInput.action.performed -= OnRotatePreformed;
    }
}
