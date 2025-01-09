using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPos, Vector3 surfaceDirection);
    void UpdateState(Vector3Int gridPos, Vector3 surfaceDirection);
}