using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPos, Vector3 forward);
    void UpdateState(Vector3Int gridPos, Vector3 forward);
}