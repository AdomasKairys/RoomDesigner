using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects = new();
    public void AddObjectAt(Vector3Int gridPos,
                            List<Vector3Int> shapeOffsets,
                            int Id,
                            int placedObjectIndex)
    {
        List<Vector3Int> occupiedPos = CalculatePositions(gridPos, shapeOffsets);
        PlacementData data = new PlacementData(occupiedPos, Id, placedObjectIndex);
        foreach (var pos in occupiedPos)
        {
            if (placedObjects.ContainsKey(pos))
                return; //throw
            placedObjects[pos] = data;
        }
    }
    public bool CanPlaceObjectAt(Vector3Int gridPos, List<Vector3Int> shapeOffsets, int[] ignoreObjectsIndex)
    {
        List<Vector3Int> posToOccupy = CalculatePositions(gridPos, shapeOffsets);

        return !posToOccupy.Any(pos => placedObjects.Where((p)=> !ignoreObjectsIndex.Contains(p.Value.placedObjectIndex)).ToDictionary((k)=>k.Key, (v)=>v.Value).ContainsKey(pos)) && !IsOutOfBounds(posToOccupy);
    }

    public int GetIndex(Vector3Int gridPos, Vector3 direction)
    {
        var placementData = GetPlacementDataAt(gridPos, direction);
        if(placementData == null) return -1;
        return placementData.placedObjectIndex;
    }
    public int GetId(Vector3Int gridPos, Vector3 direction)
    {
        var placementData = GetPlacementDataAt(gridPos, direction);
        if (placementData == null) return -1;
        return placementData.id;
    }
    public void RemoveObjectAt(Vector3Int gridPos)
    {
        foreach (var pos in placedObjects[gridPos].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
    public void RemoveObjectByIndex(int index)
    {
        var placedObject = placedObjects.FirstOrDefault((p) => p.Value.placedObjectIndex == index);
        foreach (var pos in placedObject.Value.occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
    private PlacementData GetPlacementDataAt(Vector3Int gridPos, Vector3 direction)
    {
        var newGridPos = Vector3Int.FloorToInt(gridPos + direction * 0.5f);

        if (!placedObjects.ContainsKey(newGridPos))
            return null;

        return placedObjects[newGridPos];
    }
    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, List<Vector3Int> shapeOffsets)
    {
        List<Vector3Int> returnVal = new();
        foreach (var offset in shapeOffsets)
        {
            returnVal.Add(gridPos + offset);
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
[System.Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int id;
    public int placedObjectIndex;
    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.id = id;
        this.placedObjectIndex = placedObjectIndex;
    }
}
