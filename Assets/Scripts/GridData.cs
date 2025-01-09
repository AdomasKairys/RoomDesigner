using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

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
            if (placedObjects.ContainsKey(pos))
                return; //throw
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, List<Vector3Int> shapeOffsets, Vector3 anchor) //generalize for walls and rotation
    {
        List<Vector3Int> returnVal = new();
        foreach (var offset in shapeOffsets)
        {
            var adjsutedGridPos = GridPositionRealativeToAnchor(gridPos + offset, anchor);
            returnVal.Add(adjsutedGridPos);
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPos, List<Vector3Int> shapeOffsets, Vector3 anchor)
    {
        List<Vector3Int> posToOccupy = CalculatePositions(gridPos, shapeOffsets, anchor);

        return !posToOccupy.Any(pos => placedObjects.ContainsKey(pos));
    }
    public Vector3Int GridPositionRealativeToAnchor(Vector3Int gridPos, Vector3 anchor, float gridCellSize = 1f)
    {
        var newGridPos = gridPos + anchor * (gridCellSize / 2);
        return Vector3Int.FloorToInt(newGridPos);
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
