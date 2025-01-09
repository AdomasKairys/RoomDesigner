using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> _placedObjects = new();

    public void AddObjectAt(Vector3Int gridPos,
                            List<Vector3Int> shapeOffsets,
                            Vector3 anchor,
                            int Id,
                            int placedObjectIndex)
    {
        List<Vector3Int> occupiedPos = CalculatePositions(gridPos, shapeOffsets, anchor);
        PlacementData data = new PlacementData(occupiedPos, Id, placedObjectIndex);
        foreach (var pos in occupiedPos)
        {
            if (_placedObjects.ContainsKey(pos))
                return; //throw
            _placedObjects[pos] = data;
        }
    }
    public bool CanPlaceObjectAt(Vector3Int gridPos, List<Vector3Int> shapeOffsets, Vector3 anchor)
    {
        List<Vector3Int> posToOccupy = CalculatePositions(gridPos, shapeOffsets, anchor);

        return !posToOccupy.Any(pos => _placedObjects.ContainsKey(pos)) && !IsOutOfBounds(posToOccupy);
    }
    public Vector3Int GridPositionRealativeToAnchor(Vector3Int gridPos, Vector3 anchor, float gridCellSize = 1f)
    {
        var newGridPos = gridPos + anchor * (gridCellSize / 2);
        return Vector3Int.FloorToInt(newGridPos);
    }

    public int GetIndex(Vector3Int gridPos, Vector3 anchor)
    {
        var adjustedGridPos = GridPositionRealativeToAnchor(gridPos, anchor);
        Debug.Log("anchor " + anchor);
        Debug.Log(adjustedGridPos);
        foreach (var pos in _placedObjects)
            Debug.Log(pos);

        if (!_placedObjects.ContainsKey(adjustedGridPos))
            return -1;
        return _placedObjects[adjustedGridPos].PlacedObjectIndex;
    }
    public void RemoveObjectAt(Vector3Int gridPos, Vector3 anchor)
    {
        var adjustedGridPos = GridPositionRealativeToAnchor(gridPos, anchor);
        foreach (var pos in _placedObjects[adjustedGridPos].occupiedPositions)
        {
            _placedObjects.Remove(pos);
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, List<Vector3Int> shapeOffsets, Vector3 anchor) //generalize for walls and rotation
    {
        List<Vector3Int> returnVal = new();
        foreach (var offset in shapeOffsets)
        {
            var adjustedGridPos = GridPositionRealativeToAnchor(gridPos + offset, anchor);
            returnVal.Add(adjustedGridPos);
        }
        return returnVal;
    }
    private bool IsOutOfBounds(List<Vector3Int> posToOccupy)
    {
        // max bounds have to be one less than the actual bounds becouse of the way the grid position works
        // needs to be changed if the grid scale changes
        int[] xBounds = { -5, 4 };
        int[] yBounds = { 0, 4 };
        int[] zBounds = { -10, -1 };

        return posToOccupy.Any((pos) =>
                                (pos.x - xBounds[0]) * (xBounds[1] - pos.x) < 0 ||
                                (pos.y - yBounds[0]) * (yBounds[1] - pos.y) < 0 ||
                                (pos.z - zBounds[0]) * (zBounds[1] - pos.z) < 0);
    }
    
}
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int Id { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupidePositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupidePositions;
        Id = id;
        PlacedObjectIndex = placedObjectIndex;
    }
}
